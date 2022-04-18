#pragma once

#include "core/configured_boost_thread.h"

class Unit;

namespace potato
{
	class Area;

	class AreaTransporter final
	{
	public:
		boost::future<bool> transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit);
	};
}
