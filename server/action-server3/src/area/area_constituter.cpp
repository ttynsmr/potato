#include "area_constituter.h"

#include <string>

#include "area/area.h"
#include "area/area_registry.h"
#include "node/node.h"
#include "services/service_registry.h"
#include "services/game_service_provider.h"

using namespace potato;

AreaConsituter::AreaConsituter()
{
}

AreaConsituter::~AreaConsituter()
{
}

boost::future<bool> AreaConsituter::load(std::shared_ptr<Area> area, const std::string& file)
{
	return boost::async(boost::launch::async, [currentArea = std::move(area)]{
		// load area resources
		switch (currentArea->getAreaId().value_of())
		{
			case 0:
				{
					auto node = std::make_shared<Node>();
					auto trigger = node->addComponent<TriggerableComponent>(node);
					trigger->position = Eigen::Vector3f(5, 0, 0);
					trigger->offset = Eigen::Vector3f(-0.5f, -0.5f, -0.5f);
					trigger->size = Eigen::Vector3f(1, 1, 1);
					trigger->setOnTrigger([currentArea](auto unit, auto now) {
						// TODO: Move unit to another area(1)
						auto gameServiceProvider = ServiceRegistry::instance().findServiceProvider<GameServiceProvider>();
						auto areaRegistory = gameServiceProvider->getAreaRegistry();
						auto nextArea = areaRegistory->getArea(AreaId(1));

						if (!nextArea)
						{
							nextArea = areaRegistory->addArea(AreaId(1));
							nextArea->requestLoad();
						}

						areaRegistory->transportUnit(currentArea, nextArea, unit);
						});
					currentArea->getNodeRoot()->addNode(node);
				}
				break;
			case 1:
				{
					auto node = std::make_shared<Node>();
					auto trigger = node->addComponent<TriggerableComponent>(node);
					trigger->position = Eigen::Vector3f(-5, 0, 0);
					trigger->offset = Eigen::Vector3f(-0.5f, -0.5f, -0.5f);
					trigger->size = Eigen::Vector3f(1, 1, 1);
					trigger->setOnTrigger([currentArea](auto unit, auto now) {
						// TODO: Move unit to another area(0)
						auto gameServiceProvider = ServiceRegistry::instance().findServiceProvider<GameServiceProvider>();
						auto areaRegistory = gameServiceProvider->getAreaRegistry();
						auto nextArea = areaRegistory->getArea(AreaId(0));

						if (!nextArea)
						{
							nextArea = areaRegistory->addArea(AreaId(0));
							nextArea->requestLoad();
						}
						
						areaRegistory->transportUnit(currentArea, nextArea, unit);
						});
					currentArea->getNodeRoot()->addNode(node);
				}
				break;
		}
		return true;
	});
}


boost::future<bool> AreaConsituter::unload(std::shared_ptr<Area> area)
{
}