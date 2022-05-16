#include "area.h"

#include "core/configured_boost_thread.h"

#include "units/unit.h"
#include "units/components/npc_component.h"

#include "node/node.h"

#include "area/area_constituter.h"

namespace potato
{
	Area::Area(AreaId areaId)
		: _areaId(areaId)
		, _nodeRoot(std::make_shared<NodeRoot>()
		) {}

	Area::~Area() {}

	void Area::requestLoad()
	{
		if (loaded)
		{
			return;
		}

		assert(!asyncOperating);
		assert(!futureForLoading.valid());
		assert(!futureForUnloading.valid());
		asyncOperating = true;

		AreaConsituter arecon;
		futureForLoading = arecon.load(shared_from_this(), "").then([shared_this = std::move(shared_from_this())](auto f) {
			shared_this->asyncOperating = false;
			return f.get();
			});
		futureForLoading.wait();
		loaded = true;
	}

	void Area::requestUnload()
	{
		if (!loaded)
		{
			return;
		}

		assert(!asyncOperating);
		assert(!futureForLoading.valid());
		assert(!futureForUnloading.valid());
		asyncOperating = true;

		// TODO: Remove units from the area

		futureForUnloading = unload().then([shared_this = std::move(shared_from_this())](auto f) {
			shared_this->asyncOperating = false;
			return f.get();
		});
		futureForUnloading.wait();
		loaded = false;
	}

	boost::future<bool> Area::unload()
	{
		return boost::async(boost::launch::async, [shared_this = std::move(shared_from_this())]{
			shared_this->_nodeRoot->clearNodes();
			shared_this->_nodeRoot->clearComponents();
		return true;
		});
	}

	// void Area::sendDynamicTriggers(std::shared_ptr<Unit> unit) {}

	void Area::enter(std::shared_ptr<Unit> unit)
	{
		fmt::print("unit:{} enter area:{}\n", unit->getUnitId(), getAreaId());
		unit->setAreaId(getAreaId());
		std::scoped_lock l(_unitsMutex);
		_units.push_back(unit);
		_sessionIds.emplace(unit->getSessionId());

		const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		unit->onEnterArea(now, _areaId);
	}

	void Area::leave(std::shared_ptr<Unit> leaveUnit)
	{
		fmt::print("unit:{} leave area:{}\n", leaveUnit->getUnitId(), getAreaId());
		const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
		leaveUnit->onLeaveArea(now, _areaId);
		_sessionIds.erase(leaveUnit->getSessionId());
		std::scoped_lock l(_unitsMutex);
		_units.remove_if([leaveUnit = std::move(leaveUnit)](auto& u)
			{
				auto unit = u.lock();
				if (unit == nullptr)
				{
					return true;
				}
				
				return leaveUnit->getUnitId() == unit->getUnitId();
			});
	}

	void Area::update(time_t now)
	{
		_nodeRoot->process([this, now](std::shared_ptr<Node> node)
			{
				auto trigger = node->getComponent<TriggerableComponent>();
				if (trigger)
				{
					process([trigger, now](std::shared_ptr<Unit> unit)
						{
							if (unit->hasComponent<NpcComponent>())
							{
								return;
							}

							if (trigger->containsAABB(unit->getPosition()))
							{
								trigger->invokeOnTrigger(unit, now);
							}
						});
				}
			});
	}

	const std::set<potato::net::SessionId> Area::getSessionIds() const
	{
		return _sessionIds;
	}

	void Area::process(Processor processor)
	{
		std::scoped_lock l(_unitsMutex);
		std::for_each(_units.begin(), _units.end(), [&processor](auto weakUnit)
			{
				auto unit = weakUnit.lock();
				if (!unit)
				{
					return;
				}
				processor(unit);
			});
	}

	std::shared_ptr<NodeRoot> Area::getNodeRoot()
	{
		return _nodeRoot;
	}
}
