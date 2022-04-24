#pragma once

#include <list>
#include <set>
#include <memory>
#include <future>

#include "area_types.h"
#include "session/session_types.h"

#define BOOST_THREAD_PROVIDES_FUTURE
#define BOOST_THREAD_PROVIDES_FUTURE_CONTINUATION
#include <boost/thread/future.hpp>

class Unit;

namespace potato
{
	class NodeRoot;

	class Area final
		: public std::enable_shared_from_this<Area>
	{
	public:
		Area(AreaId areaId);
		~Area();

		AreaId getAreaId() const { return _areaId; }

		void requestLoad();
		void requestUnload();

		void enter(std::shared_ptr<Unit> unit);
		void leave(std::shared_ptr<Unit> unit);

		void update(time_t now);

		const std::set<potato::net::SessionId> getSessionIds() const;

		using Processor = std::function<void(std::weak_ptr<Unit>& weakUnit)>;
		void process(Processor processor);

		std::shared_ptr<NodeRoot> getNodeRoot();

	private:
		boost::future<bool> load();
		boost::future<bool> unload();

		std::atomic_bool asyncOperating = false;
		boost::future<bool> futureForLoading;
		boost::future<bool> futureForUnloading;
		const AreaId _areaId;
		std::list<std::weak_ptr<Unit>> _units;
		std::set<potato::net::SessionId> _sessionIds;
		std::shared_ptr<NodeRoot> _nodeRoot;
	};
}
