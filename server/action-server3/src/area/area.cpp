#include "area.h"

#include "units/unit.h"
#include "node/node.h"

namespace potato
{
	Area::Area(AreaId areaId)
		: _areaId(areaId) {}

	Area::~Area() {}

	void Area::requestLoad()
	{
		assert(!asyncOperating);
		assert(futureForLoading.valid());
		assert(futureForUnloading.valid());
		asyncOperating = true;
		futureForLoading = load();
	}

	void Area::requestUnoad()
	{
		assert(!asyncOperating);
		assert(futureForLoading.valid());
		assert(futureForUnloading.valid());
		asyncOperating = true;

		// TODO: Remove units from the area

		futureForUnloading = unload();
	}

	std::future<bool> Area::load()
	{
		return std::async(std::launch::async, [shared_this = std::move(shared_from_this())]{
			// load area resources
			auto node = std::make_shared<Node>();
			auto trigger = node->addComponent<TriggerableComponent>(node);
			trigger->position = Eigen::Vector3f(5, 0, 0);
			trigger->offset = Eigen::Vector3f(-0.5f, -0.5f, -0.5f);
			trigger->size = Eigen::Vector3f(1, 1, 1);
			trigger->setOnTrigger([](auto unit) {
				// TODO: Move unit to another area
				});
			shared_this->_nodeRoot->addNode(node);
			return true;
		});
	}

	std::future<bool> Area::unload()
	{
		return std::async(std::launch::async, [] { return true; });
	}

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

	void Area::update()
	{
		if (futureForLoading.valid())
		{
			if (futureForLoading.get())
			{
				// load success
			}
			else
			{
				// load fail
			}
			asyncOperating = false;
		}

		if (futureForUnloading.valid())
		{
			if (futureForUnloading.get())
			{
				// unload success
			}
			else
			{
				// unload fail
			}
			asyncOperating = false;
		}

		_nodeRoot->process([this](std::shared_ptr<Node> node) {
			auto trigger = node->getComponent<TriggerableComponent>();
			if (trigger)
			{
				process([trigger](auto weakUnit) {
					auto unit = weakUnit.lock();
					if (trigger->containsAABB(unit->getPosition()))
					{
						trigger->invokeOnTrigger(unit);
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
}
