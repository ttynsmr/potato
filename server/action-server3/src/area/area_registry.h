#pragma once

#include "area_types.h"

namespace potato
{
	class Area;

	class AreaRegistry final
	{
	public:
		AreaRegistry();
		~AreaRegistry();

		std::shared_ptr<Area> addArea(AreaId areaId);
		std::shared_ptr<Area> getArea(AreaId areaId);

	private:
		void removeArea(AreaId areaId);

		std::unordered_map<AreaId, std::shared_ptr<Area>> _areas;
	};
}
