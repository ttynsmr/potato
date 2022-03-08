#pragma once

#include <memory>
#include <list>

#include "unit_types.h"
#include "session/session_types.h"

class Unit;

namespace potato
{
	class UnitRegistory
	{
	public:
		UnitRegistory();
		~UnitRegistory();

		std::shared_ptr<Unit> createUnit(net::SessionId sessionId);
		void unregisterUnit(std::shared_ptr<Unit> unit);
		void unregisterUnitByUnitId(UnitId unitId);
		void unregisterUnitBySessionId(net::SessionId sessionId);

		const std::list<std::shared_ptr<Unit>> getUnits() const { return units; }

	private:
		UnitId generateUnitId();
		UnitId currentUnitId = 0;
		std::list<std::shared_ptr<Unit>> units;
	};
}
