#include "unit_registry.h"

#include <memory>

#include "unit_types.h"
#include "units/unit.h"

namespace potato
{
	UnitRegistry::UnitRegistry() {}
	UnitRegistry::~UnitRegistry() {}

	std::shared_ptr<Unit> UnitRegistry::createUnit(net::SessionId sessionId)
	{
		auto unit = std::make_shared<Unit>(generateUnitId(), sessionId);
		units.emplace_back(unit);
		return unit;
	}

	void UnitRegistry::unregisterUnit(std::shared_ptr<Unit> unit)
	{
		units.remove(unit);
	}

	void UnitRegistry::unregisterUnitByUnitId(UnitId unitId)
	{
		units.remove_if([unitId](auto& u) { return u->getUnitId() == unitId; });
	}

	void UnitRegistry::unregisterUnitBySessionId(net::SessionId sessionId)
	{
		units.remove_if([sessionId](auto& u) { return u->getSessionId() == sessionId; });
	}

	const std::shared_ptr<Unit> UnitRegistry::findUnitBySessionId(net::SessionId sessionId) const
	{
		auto unitIt = std::find_if(units.begin(), units.end(), [sessionId](auto& unit) { return unit->getSessionId() == sessionId; });
		if (unitIt == units.end())
		{
			return nullptr;
		}
		return *unitIt;
	}

	const std::shared_ptr<Unit> UnitRegistry::findUnitByUnitId(UnitId unitId) const
	{
		auto unitIt = std::find_if(units.begin(), units.end(), [unitId](auto& unit) { return unit->getUnitId() == unitId; });
		if (unitIt == units.end())
		{
			return nullptr;
		}
		return *unitIt;
	}

	UnitId UnitRegistry::generateUnitId()
	{
		return ++currentUnitId;
	}
}
