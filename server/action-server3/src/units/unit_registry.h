#pragma once

#include <memory>
#include <list>

#include "unit_types.h"
#include "session/session_types.h"

class Unit;

namespace potato
{
	class UnitRegistry
	{
	public:
		UnitRegistry();
		~UnitRegistry();

		std::shared_ptr<Unit> createUnit(net::SessionId sessionId);
		void unregisterUnit(std::shared_ptr<Unit> unit);
		void unregisterUnitByUnitId(UnitId unitId);
		void unregisterUnitBySessionId(net::SessionId sessionId);

		const std::shared_ptr<Unit> findUnitBySessionId(net::SessionId sessionId) const;
		const std::shared_ptr<Unit> findUnitByUnitId(UnitId unitId) const;

		void update(time_t now);

		using Predecate = std::function<bool(std::shared_ptr<Unit>& unit)>;
		std::shared_ptr<Unit> find(Predecate predecate);

		using Processor = std::function<void(std::shared_ptr<Unit> unit)>;
		void process(Processor processor);

	private:
		UnitId generateUnitId();
		UnitId currentUnitId = UnitId(0);
		std::list<std::shared_ptr<Unit>> units;
	};
}
