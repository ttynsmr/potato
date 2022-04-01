#include "npc_component.h"

#include <random>

#include "core/configured_eigen.h"

#include "services/game_service_provider.h"
#include "units/unit.h"

NpcComponent::NpcComponent(std::shared_ptr<GameServiceProvider> gameServiceProvider) : _gameServiceProvider(gameServiceProvider)
{
	std::uniform_int_distribution<int64_t> dist(0, 5000);
	_timeScattering = dist(_gameServiceProvider.lock()->getRandomEngine());
}

NpcComponent::~NpcComponent() {}

void NpcComponent::update(std::shared_ptr<Unit> unit, int64_t now)
{
	auto getMoveDirection = [](const Eigen::Vector3f& direction) {
		if (direction.y() == 0)
		{
			return direction.x() > 0 ? potato::UnitDirection::UNIT_DIRECTION_RIGHT : potato::UnitDirection::UNIT_DIRECTION_LEFT;
		}
		else
		{
			return direction.y() < 0 ? potato::UnitDirection::UNIT_DIRECTION_DOWN : potato::UnitDirection::UNIT_DIRECTION_UP;
		}
	};

	auto gameServiceProvider = _gameServiceProvider.lock();
	if (!unit->isMoving() && (((now + _timeScattering) / 1000) % 5) == 0)
	{
		auto moveCommand = std::make_shared<MoveCommand>();
		moveCommand->startTime = now;
		const auto from = unit->getTrackbackPosition(now);
		Eigen::Vector3f randomDirection;

		if ((((now + _timeScattering) / 1000) % 15) != 0)
		{
			std::uniform_real_distribution<float> distr(-1, 1);
			randomDirection << distr(gameServiceProvider->getRandomEngine()), distr(gameServiceProvider->getRandomEngine()), 0;
			randomDirection.normalize();
		}
		else
		{
			randomDirection = -from.normalized();
		}

		const auto to = from + randomDirection * 500;
		moveCommand->from = from;
		moveCommand->to = to;
		moveCommand->speed = 0.0025f;
		moveCommand->direction = getMoveDirection(randomDirection);
		moveCommand->moveId = 0;
		unit->inputCommand(moveCommand);
		gameServiceProvider->sendMove(potato::net::SessionId(0), unit, moveCommand);

		auto stopCommand = std::make_shared<StopCommand>();
		stopCommand->stopTime = now + 2000;
		stopCommand->direction = moveCommand->direction;
		stopCommand->moveId = 0;
		unit->inputCommand(stopCommand);
		gameServiceProvider->sendStop(potato::net::SessionId(0), unit, stopCommand);

		//const auto expectStop = moveCommand->getPosition(stopCommand->stopTime);
		//fmt::print("from: {}, {}, {}  to:{}, {}, {}  expectStop:{}, {}, {}\n",
		//	from.x(), from.y(), from.z(),
		//	to.x(), to.y(), to.z(),
		//	expectStop.x(), expectStop.y(), expectStop.z()
		//);
	}
}
