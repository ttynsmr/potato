#pragma once

#include <mutex>

#include "area_types.h"

class Unit;

namespace potato
{
	class Area;
	class AreaTransporter;

	class AreaRegistry final
	{
	public:
		AreaRegistry();
		~AreaRegistry();

		std::shared_ptr<Area> addArea(AreaId areaId);
		std::shared_ptr<Area> getArea(AreaId areaId);

		std::shared_ptr<AreaTransporter> transportUnit(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit, time_t now);

		void update(time_t now);

	private:
		void removeArea(AreaId areaId);

		std::unordered_map<AreaId, std::shared_ptr<Area>> _areas;
		std::recursive_mutex _areasMutex;
	};
}
