#pragma once

#include <memory>

#include "core/configured_boost_thread.h"

namespace potato
{
	class Area;

	class AreaConsituter final
		: public std::enable_shared_from_this<Area>
	{
	public:
		AreaConsituter();
		~AreaConsituter();

		boost::future<bool> load(std::shared_ptr<Area> area, const std::string& file);
		boost::future<bool> unload(std::shared_ptr<Area> area);
	};
}
