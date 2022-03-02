#include "unit.h"
#include <gml/gml.hpp>
#include <chrono>

Unit::Unit(UnitId unitId) : unitId(unitId) {}

std::shared_ptr<ICommand> Unit::getLastCommand()
{
	return history.size() > 0 ? history.back() : nullptr;
}

void Unit::inputCommand(std::shared_ptr<ICommand> command)
{
	auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
	if (stopCommand != nullptr)
	{
		stopCommand->lastMoveCommand = std::dynamic_pointer_cast<MoveCommand>(getLastCommand());
	}

	inputQueue.emplace(command);
}

void Unit::update(int64_t now)
{
	std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();

	auto updatePosition = [this](std::shared_ptr<MoveCommand> currentMove, int64_t now) {
		auto distance = gml::length(currentMove->to - currentMove->from);
		auto progress = (now - currentMove->startTime) / (distance / currentMove->speed);
		//Debug.Log($"distance:{distance}, progress:{progress}, estimate time:{(distance / currentMove.Speed)}");
		Positoin = gml::mix(currentMove->from, currentMove->to, progress);
		std::cout << "time: " << now << " position[" << currentMove->moveId << "]: x:" << Positoin[0] << " y:" << Positoin[1] << " z:" << Positoin[2] << "\n";
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
}
