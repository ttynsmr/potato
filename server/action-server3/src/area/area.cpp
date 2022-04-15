#include "area.h"

#include "units/unit.h"

namespace potato
{
	Area::Area(AreaId areaId)
		: _areaId(areaId) {}

	Area::~Area() {}

	void Area::enter(std::shared_ptr<Unit> unit)
	{
		unit->setAreaId(getAreaId());
		_units.push_back(unit);
		_sessionIds.emplace(unit->getSessionId());
	}

	void Area::leave(std::shared_ptr<Unit> unit)
	{
		_sessionIds.erase(unit->getSessionId());
		_units.remove_if([unit](auto& u) { return unit->getUnitId() == u.lock()->getUnitId(); });
	}

	void Area::update()
	{
	}

	const std::set<potato::net::SessionId> Area::getSessionIds() const
	{
		return _sessionIds;
	}

	void Area::process(Processor processor)
	{
		std::for_each(_units.begin(), _units.end(), processor);
	}
}
