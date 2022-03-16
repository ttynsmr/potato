#include "unit.h"
#include <chrono>
#include <iostream>
#include <memory>
#include <fmt/core.h>

Unit::Unit(UnitId unitId, potato::net::SessionId sessionId)
	: _unitId(unitId), _sessionId(sessionId)
{
	setPosition({ 0, 0, 0 });
}

std::shared_ptr<ICommand> Unit::getLastCommand()
{
	return history.size() > 0 ? history.back() : nullptr;
}

std::shared_ptr<MoveCommand> Unit::getLastMoveCommand()
{
	auto lastCommand = getLastCommand();
	auto lastCommandAsStopCommand = std::dynamic_pointer_cast<StopCommand>(lastCommand);
	if (lastCommandAsStopCommand != nullptr)
	{
		return lastCommandAsStopCommand->lastMoveCommand.lock();
	}
	else
	{
		return std::dynamic_pointer_cast<MoveCommand>(lastCommand);
	}
}

const std::shared_ptr<MoveCommand> Unit::getLastMoveCommand() const
{
	return getLastMoveCommand();
}

void Unit::inputCommand(std::shared_ptr<ICommand> command)
{
	auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
	if (stopCommand != nullptr)
	{
		stopCommand->lastMoveCommand = std::dynamic_pointer_cast<MoveCommand>(getLastCommand());
		_isMoving = false;
	}
	else
	{
		_isMoving = true;
	}

	inputQueue.emplace(command);
}

Eigen::Vector3f lerp(const Eigen::Vector3f& from, const Eigen::Vector3f& to, float progress)
{
	return (to - from) * progress + from;
}

void Unit::update(int64_t now)
{
	auto updatePosition = [this](std::shared_ptr<MoveCommand> currentMove, int64_t now) {
		auto distance = (currentMove->to - currentMove->from).norm();
		auto progress = std::min(1.0f, (now - currentMove->startTime) / (distance / currentMove->speed));
		position = lerp(currentMove->from, currentMove->to, progress);
		_direction = currentMove->direction;
		//fmt::print("unit[{}] time: {} position[{}]: x:{} y:{} z:{} direction:{}\n", unitId, now, currentMove->moveId, position.x(), position.y(), position.z(), currentMove->direction);
	};

	while (simulatedNow < now)
	{
		if (currentMove == nullptr)
		{
			auto lastCommand = history.size() > 0 ? history.back() : nullptr;
			if (lastCommand != nullptr)
			{
				if (typeid(lastCommand) != typeid(std::shared_ptr<StopCommand>))
				{
					auto last = history.back();
					auto stopCommand = std::dynamic_pointer_cast<StopCommand>(last);
					updatePosition(stopCommand->lastMoveCommand.lock(), stopCommand->stopTime);
					if (inputQueue.size() == 0)
					{
						break;
					}
				}
			}
		}
		else
		{
			updatePosition(currentMove, simulatedNow);
		}

		if (inputQueue.size() > 0 && inputQueue.front()->getActionTime() <= now)
		{
			auto command = inputQueue.front();

			if (currentMove != nullptr && currentMove->getCommandType() == CommandType::KnockBack)
			{
				auto knockbackMove = std::dynamic_pointer_cast<KnockbackCommand>(currentMove);
				if (command->getActionTime() >= knockbackMove->getGoalTime())
				{
					// ok
					fmt::print("unit[{}] knockback!!                until {}(.. {}sec), command action time is {}.\n", _unitId, knockbackMove->endTime, (knockbackMove->endTime - simulatedNow) / 1000.0, command->getActionTime());
				}
				else
				{
					// blocked
					fmt::print("unit[{}] knockback!! input dropping until {}(.. {}sec), command action time is {}.\n", _unitId, knockbackMove->endTime, (knockbackMove->endTime - simulatedNow) / 1000.0, command->getActionTime());
					fmt::print("unit[{}] now:{} simulationNow:{} endTime:{} actionTime:{}\n", _unitId, now, now - simulatedNow, knockbackMove->endTime - simulatedNow, command->getActionTime() - simulatedNow);
					fmt::print("unit[{}] CommandType:{} dropped\n", _unitId, static_cast<int>(command->getCommandType()));
					simulatedNow = now;
					break;
				}
			}
			inputQueue.pop();

			switch (command->getCommandType())
			{
			case CommandType::Move:
			case CommandType::KnockBack:
				{
					auto moveCommand = std::dynamic_pointer_cast<MoveCommand>(command);
					simulatedNow = moveCommand->startTime;
					moveCommand->lastMoveCommand = currentMove;
					history.emplace_back(moveCommand);
					currentMove = moveCommand;
				}
				break;
			case CommandType::Stop:
				{
					auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
					simulatedNow = stopCommand->stopTime;
					stopCommand->lastMoveCommand = currentMove;
					history.emplace_back(stopCommand);
					currentMove = nullptr;
				}
				break;
			}
		}
		else
		{
			simulatedNow = now;
		}
	}

	while (history.size() > 2)
	{
		if (!history.front()->isExpired(now))
		{
			break;
		}

		//fmt::print("remove old history\n");
		history.pop_front();
	}
}

Eigen::Vector3f Unit::getTrackbackPosition(int64_t now) const
{
	if (history.empty())
	{
		return Eigen::Vector3f();
	}
	auto past = history.begin();
	auto next = ++past;
	while (next != history.end())
	{
		if ((*next)->getActionTime() > now)
		{
			break;
		}

		past = next++;
	}

	auto lastMoveCommand = std::dynamic_pointer_cast<MoveCommand>((*past));
	long lastActionTime = now;
	if (lastMoveCommand == nullptr)
	{
		auto stopCommand = std::dynamic_pointer_cast<StopCommand>((*past));
		lastMoveCommand = stopCommand->lastMoveCommand.lock();
		lastActionTime = stopCommand->getActionTime();
	}

	auto distance = (lastMoveCommand->to - lastMoveCommand->from).norm();
	auto progress = lastMoveCommand->speed > 0 ? std::min(1.0f, (lastActionTime - lastMoveCommand->startTime) / (distance / lastMoveCommand->speed)) : 0;
	return lerp(lastMoveCommand->from, lastMoveCommand->to, progress);
}

Eigen::Vector3f Unit::getCurrentPosition() const
{
	return position;
}

void Unit::setPosition(const Eigen::Vector3f& position)
{
	history.clear();

	auto moveCommand = std::make_shared<MoveCommand>();
	moveCommand->startTime = 0;
	moveCommand->from = position;
	moveCommand->to = position;
	moveCommand->speed = 0;
	moveCommand->direction = potato::UnitDirection::UNIT_DIRECTION_DOWN;
	moveCommand->moveId = 0;

	auto stopCommand = std::make_shared<StopCommand>();
	stopCommand->lastMoveCommand = moveCommand;
	stopCommand->stopTime = 0;
	stopCommand->direction = potato::UnitDirection::UNIT_DIRECTION_DOWN;
	stopCommand->moveId = 0;

	history.emplace_back(moveCommand);
	history.emplace_back(stopCommand);
}
