#include "unit.h"
#include <chrono>
#include <iostream>
#include <memory>
#include <fmt/core.h>

Unit::Unit(UnitId unitId, SessionId sessionId)
	: unitId(unitId), sessionId(sessionId)
{
	auto moveCommand = std::make_shared<MoveCommand>();
	moveCommand->startTime = 0;
	moveCommand->from = {};
	moveCommand->to = {};
	moveCommand->speed = 0;
	moveCommand->direction = {};
	moveCommand->moveId = 0;

	auto stopCommand = std::make_shared<StopCommand>();
	stopCommand->lastMoveCommand = moveCommand;
	stopCommand->stopTime = 0;
	stopCommand->direction = {};
	stopCommand->moveId = 0;

	history.emplace_back(moveCommand);
	history.emplace_back(stopCommand);
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

void Unit::update(int64_t now)
{
	auto updatePosition = [this](std::shared_ptr<MoveCommand> currentMove, int64_t now) {
		auto distance = (currentMove->to - currentMove->from).norm();
		auto progress = std::min(1.0f, (now - currentMove->startTime) / (distance / currentMove->speed));
		position = (currentMove->to - currentMove->from) * progress + currentMove->from;
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

		if (inputQueue.size() > 0)
		{
			auto command = inputQueue.front();
			inputQueue.pop();
			if (std::dynamic_pointer_cast<MoveCommand>(command) != nullptr)
			{
				auto moveCommand = std::dynamic_pointer_cast<MoveCommand>(command);
				simulatedNow = moveCommand->startTime;
				moveCommand->lastMoveCommand = currentMove;
				history.emplace_back(moveCommand);
				currentMove = moveCommand;
			}
			else if(std::dynamic_pointer_cast<StopCommand>(command) != nullptr)
			{
				auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
				simulatedNow = stopCommand->stopTime;
				stopCommand->lastMoveCommand = currentMove;
				history.emplace_back(stopCommand);
				currentMove = nullptr;
			}
		}
		else
		{
			simulatedNow = now;
		}
	}

	while (!history.empty())
	{
		if (!history.front()->isExpired(now))
		{
			break;
		}

		fmt::print("remove old history\n");
		history.pop_front();
	}
}
