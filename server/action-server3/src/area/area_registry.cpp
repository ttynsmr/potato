#include "area_registry.h"

#include <fmt/core.h>

#include "area/area.h"
#include "area/area_transporter.h"

#include "units/unit.h"

using namespace potato;

AreaRegistry::AreaRegistry()
{
}

AreaRegistry::~AreaRegistry()
{
}

std::shared_ptr<Area> AreaRegistry::addArea(AreaId areaId)
{
	std::scoped_lock l(_areasMutex);
	
	auto found = _areas.find(areaId);
	if (found != _areas.end())
	{
		return found->second;
	}
	
	auto area = std::make_shared<Area>(areaId);
	_areas.emplace(area->getAreaId(), area);
	return area;
}

void AreaRegistry::removeArea(AreaId areaId)
{
	std::scoped_lock l(_areasMutex);
	_areas.erase(areaId);
}

std::shared_ptr<Area> AreaRegistry::getArea(AreaId areaId)
{
	std::scoped_lock l(_areasMutex);
	auto areaIt = _areas.find(areaId);
	if (areaIt == _areas.end())
	{
		//return addArea(areaId);
		return nullptr;
	}
	else
	{
		return areaIt->second;
	}
}

std::shared_ptr<AreaTransporter> AreaRegistry::transportUnit(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit, time_t now)
{
	fmt::print("area transport from:{} to:{} unit:{}\n", fromArea->getAreaId(), toArea->getAreaId(), unit->getUnitId());
	auto areaTransporter = std::make_shared<AreaTransporter>();
	areaTransporter->transport(fromArea, toArea, unit, now);
	return areaTransporter;
}

void AreaRegistry::update(time_t now)
{
	std::scoped_lock l(_areasMutex);
	std::for_each(_areas.begin(), _areas.end(), [&](auto& area)
		{
			area.second->update(now);
		}
	);
}
