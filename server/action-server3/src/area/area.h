#pragma once

#include <list>
#include <memory>

class Unit;

namespace potato
{
	class Area final
	{
	public:
		Area();
		~Area();

		void enter(std::shared_ptr<Unit> unit);
		void leave(std::shared_ptr<Unit> unit);

		void update();

	private:
		std::list<std::weak_ptr<Unit>> _units;
	};
}
