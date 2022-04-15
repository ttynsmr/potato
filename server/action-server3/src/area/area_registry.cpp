#include "area_registry.h"

#include "area/area.h"

using namespace potato;

AreaRegistry::AreaRegistry()
{
}

AreaRegistry::~AreaRegistry()
{
}

std::shared_ptr<Area> AreaRegistry::addArea(AreaId areaId)
{
	auto area = std::make_shared<Area>(areaId);
	_areas.emplace(area->getAreaId(), area);
	return area;
}

void AreaRegistry::removeArea(AreaId areaId)
{
	_areas.erase(areaId);
}

std::shared_ptr<Area> AreaRegistry::getArea(AreaId areaId)
{
	auto areaIt = _areas.find(areaId);
	if (areaIt == _areas.end())
	{
		return addArea(areaId);
	}
	else
	{
		return areaIt->second;
	}
}
