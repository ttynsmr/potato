#pragma once

#include "area_types.h"

namespace potato
{
	class Area;

	class AreaRegistry final
	{
		AreaRegistry();
		~AreaRegistry();

		std::unordered_map<AreaId, std::shared_ptr<Area>> _areas;
	};
}
