#pragma once

#include <boost/signals2/signal.hpp>
#include "core/configured_boost_thread.h"

class Unit;

namespace potato
{
	class Area;

	class AreaTransporter final
		: public std::enable_shared_from_this<AreaTransporter>
	{
	public:
		AreaTransporter();
		~AreaTransporter();
		void transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit, time_t now);

	private:
		boost::signals2::connection _spawnReadyRequest;
		boost::signals2::connection _transportRequest;
	};
}
