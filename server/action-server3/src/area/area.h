#pragma once

#include <list>
#include <set>
#include <memory>
#include <future>

#include "area_types.h"
#include "session/session_types.h"

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
		void requestUnoad();

		void enter(std::shared_ptr<Unit> unit);
		void leave(std::shared_ptr<Unit> unit);

		void update();

		const std::set<potato::net::SessionId> getSessionIds() const;

		using Processor = std::function<void(std::weak_ptr<Unit> weakUnit)>;
		void process(Processor processor);

	private:
		std::future<bool> load();
		std::future<bool> unload();

		std::atomic_bool asyncOperating = false;
		std::future<bool> futureForLoading;
		std::future<bool> futureForUnloading;
		const AreaId _areaId;
		std::list<std::weak_ptr<Unit>> _units;
		std::set<potato::net::SessionId> _sessionIds;
		std::shared_ptr<NodeRoot> _nodeRoot;
	};
}
