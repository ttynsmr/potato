#include "area.h"

#include "units/unit.h"

namespace potato
{
	Area::Area() {}
	Area::~Area() {}

	void Area::enter(std::shared_ptr<Unit> unit)
	{
		_units.push_back(unit);
	}

	void Area::leave(std::shared_ptr<Unit> unit)
	{
		_units.remove_if([unit](auto& u) { return unit->getUnitId() == u.lock()->getUnitId(); });
	}

	void Area::update()
	{
	}
}
