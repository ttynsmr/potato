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

	std::shared_ptr<Unit> UnitRegistry::findUnitBySessionId(net::SessionId sessionId)
	{
		return find([sessionId](auto& unit) { return unit->getSessionId() == sessionId; });
	}

	std::shared_ptr<Unit> UnitRegistry::findUnitByUnitId(UnitId unitId)
	{
		return find([unitId](auto& unit) { return unit->getUnitId() == unitId; });
	}

	std::shared_ptr<Unit> UnitRegistry::find(Predecate predecate)
	{
		auto unit = std::find_if(units.begin(), units.end(), predecate);
		if (unit == units.end())
		{
			return nullptr;
		}
		return (*unit);
	}

	void UnitRegistry::update(time_t now)
	{
		std::for_each(units.begin(), units.end(), [now](auto& unit)
			{
				unit->update(now);
			}
		);
	}

	void UnitRegistry::process(Processor processor)
	{
		std::for_each(units.begin(), units.end(), processor);
	}

	UnitId UnitRegistry::generateUnitId()
	{
		return ++currentUnitId;
	}
}
