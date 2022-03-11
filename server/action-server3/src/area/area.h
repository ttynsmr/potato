#pragma once

#include <list>
#include <memory>

#include "area_types.h"

class Unit;

namespace potato
{
	class Area final
	{
	public:
		Area(AreaId areaId);
		~Area();

		AreaId getAreaId() const { return _areaId; }

		void enter(std::shared_ptr<Unit> unit);
		void leave(std::shared_ptr<Unit> unit);

		void update();

	private:
		const AreaId _areaId;
		std::list<std::weak_ptr<Unit>> _units;
	};
}
