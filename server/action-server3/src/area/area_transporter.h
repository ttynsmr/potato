#pragma once

#include <boost/signals2/signal.hpp>
#include "core/configured_boost_thread.h"

class Unit;

namespace potato
{
	class Area;

	class AreaTransporter final
	{
	public:
		void transport(std::shared_ptr<Area> fromArea, std::shared_ptr<Area> toArea, std::shared_ptr<Unit> unit);

	private:
		boost::signals2::connection _spawnReadyRequest;
		boost::signals2::connection _transportRequest;
	};
}
