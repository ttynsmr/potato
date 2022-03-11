#include "unit_registory.h"

#include <memory>

#include "unit_types.h"
#include "units/unit.h"

namespace potato
{
	UnitRegistory::UnitRegistory() {}
	UnitRegistory::~UnitRegistory() {}

	std::shared_ptr<Unit> UnitRegistory::createUnit(net::SessionId sessionId)
	{
		auto unit = std::make_shared<Unit>(generateUnitId(), sessionId);
		units.emplace_back(unit);
		return unit;
	}

	void UnitRegistory::unregisterUnit(std::shared_ptr<Unit> unit)
	{
		units.remove(unit);
	}

	void UnitRegistory::unregisterUnitByUnitId(UnitId unitId)
	{
		units.remove_if([unitId](auto& u) { return u->getUnitId() == unitId; });
	}

	void UnitRegistory::unregisterUnitBySessionId(net::SessionId sessionId)
	{
		units.remove_if([sessionId](auto& u) { return u->getSessionId() == sessionId; });
	}

	const std::shared_ptr<Unit> UnitRegistory::findUnitBySessionId(net::SessionId sessionId) const
	{
		auto unitIt = std::find_if(units.begin(), units.end(), [sessionId](auto& unit) { return unit->getSessionId() == sessionId; });
		if (unitIt == units.end())
		{
			return nullptr;
		}
		return *unitIt;
	}

	UnitId UnitRegistory::generateUnitId()
	{
		return ++currentUnitId;
	}
}
