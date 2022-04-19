#include "area.h"

#include "core/configured_boost_thread.h"

#include "units/unit.h"
#include "node/node.h"

#include "area/area_constituter.h"

namespace potato
{
	Area::Area(AreaId areaId)
		: _areaId(areaId) {}

	Area::~Area() {}

	void Area::requestLoad()
	{
		assert(!asyncOperating);
		assert(!futureForLoading.valid());
		assert(!futureForUnloading.valid());
		asyncOperating = true;

		AreaConsituter arecon;
		futureForLoading = arecon.load(shared_from_this(), "").then([shared_this = std::move(shared_from_this())](auto f) {
			shared_this->asyncOperating = false;
			return f.get();
			});
	}

	void Area::requestUnoad()
	{
		assert(!asyncOperating);
		assert(!futureForLoading.valid());
		assert(!futureForUnloading.valid());
		asyncOperating = true;

		// TODO: Remove units from the area

		futureForUnloading = unload().then([shared_this = std::move(shared_from_this())](auto f) {
			shared_this->asyncOperating = false;
			return f.get();
		});
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
		unit->setAreaId(getAreaId());
		_units.push_back(unit);
		_sessionIds.emplace(unit->getSessionId());
	}

	void Area::leave(std::shared_ptr<Unit> unit)
	{
		_sessionIds.erase(unit->getSessionId());
		_units.remove_if([unit](auto& u) { return unit->getUnitId() == u.lock()->getUnitId(); });
	}

	void Area::update(time_t now)
	{
		_nodeRoot->process([this, now](std::shared_ptr<Node> node) {
			auto trigger = node->getComponent<TriggerableComponent>();
			if (trigger)
			{
				process([trigger, now](auto weakUnit) {
					auto unit = weakUnit.lock();
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
		std::for_each(_units.begin(), _units.end(), processor);
	}

	std::shared_ptr<NodeRoot> Area::getNodeRoot()
	{
		return _nodeRoot;
	}
}
