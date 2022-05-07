#include "unit.h"
#include <chrono>
#include <iostream>
#include <memory>
#include <fmt/core.h>

#include "commands/command_types.h"
#include "commands/command_move.h"
#include "commands/command_knockback.h"
#include "commands/command_stop.h"

Unit::Unit(UnitId unitId, potato::net::SessionId sessionId)
	: _unitId(unitId), _sessionId(sessionId)
{
	setPosition({ 0, 0, 0 });
	_simulatedNow = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
}

std::shared_ptr<ICommand> Unit::getLastCommand()
{
	return _history.size() > 0 ? _history.back() : nullptr;
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

std::shared_ptr<StopCommand> Unit::getFirstStopCommandFromQueue()
{
	for (auto& input : _inputQueue)
	{
		if (input->getCommandType() == CommandType::Stop)
		{
			return std::dynamic_pointer_cast<StopCommand>(input);
		}
	}

	return nullptr;
}

const std::shared_ptr<StopCommand> Unit::getFirstStopCommandFromQueue() const
{
	return getFirstStopCommandFromQueue();
}

void Unit::inputCommand(std::shared_ptr<ICommand> command)
{
	auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
	if (stopCommand != nullptr)
	{
		if (stopCommand->lastMoveCommand.expired())
		{
			stopCommand->lastMoveCommand = std::dynamic_pointer_cast<MoveCommand>(getLastCommand());
		}
		//assert(!stopCommand->lastMoveCommand.expired());
	}

	_inputQueue.emplace_back(command);

	if (command->getCommandType() == CommandType::KnockBack)
	{
		interveneHistory(command);
	}
}

void Unit::interveneHistory(std::shared_ptr<ICommand> interveneCommand)
{
	bool needsIntervene = getLastCommand()->getActionTime() >= interveneCommand->getActionTime();
	if(!needsIntervene)
	{
		return;
	}

	_history.erase(std::find_if(_history.begin(), _history.end(), [interveneCommand](auto command) {
		return command->getActionTime() >= interveneCommand->getActionTime();
		}), _history.end());

	//if (getLastCommand()->getCommandType() == CommandType::Stop)
	//{
	//	_currentMove.reset();
	//}
	//else
	//{
	//	_currentMove = std::dynamic_pointer_cast<MoveCommand>(getLastCommand());
	//}
}

void Unit::onEnterArea(int64_t now, potato::AreaId areaId)
{
}

void Unit::onLeaveArea(int64_t now, potato::AreaId areaId)
{
	setAreaId(potato::AreaId(0));
	resetCommands();
}

void Unit::onSpawn(int64_t now)
{
	for (auto& component : _components)
	{
		component.second->onSpawn(shared_from_this(), now);
	}
}

void Unit::onDespawn(int64_t now)
{
	for (auto& component : _components)
	{
		component.second->onDespawn(shared_from_this(), now);
	}
}

void Unit::onConnected(int64_t now)
{
	for (auto& component : _components)
	{
		component.second->onConnected(shared_from_this(), now);
	}
}

void Unit::onDisconnected(int64_t now)
{
	for (auto& component : _components)
	{
		component.second->onDisconnected(shared_from_this(), now);
	}
}

void Unit::update(int64_t now)
{
	for (auto& component : _components)
	{
		component.second->preUpdate(shared_from_this(), now);
	}

	auto updatePosition = [this](std::shared_ptr<MoveCommand> currentMove, int64_t now) {
		auto distance = (currentMove->to - currentMove->from).norm();
		auto progress = std::min(1.0f, (now - currentMove->startTime) / (distance / currentMove->speed));
		_position = lerp(currentMove->from, currentMove->to, progress);
		_direction = currentMove->direction;
		//fmt::print("unit[{}] time: {} position[{}]: x:{} y:{} z:{} direction:{}\n", unitId, now, currentMove->moveId, _position.x(), _position.y(), _position.z(), currentMove->direction);
	};

	while (_simulatedNow < now)
	{
		if (_currentMove == nullptr)
		{
			auto lastCommand = _history.size() > 0 ? _history.back() : nullptr;
			if (lastCommand != nullptr)
			{
				if (lastCommand->getCommandType() == CommandType::Stop)
				{
					auto stopCommand = std::dynamic_pointer_cast<StopCommand>(lastCommand);
					if (!stopCommand->lastMoveCommand.expired())
					{
						updatePosition(stopCommand->lastMoveCommand.lock(), stopCommand->stopTime);
					}
					if (_inputQueue.size() == 0)
					{
						_simulatedNow = now;
						break;
					}
				}
			}
			else
			{
				_currentMove = std::dynamic_pointer_cast<MoveCommand>(lastCommand);
			}
		}
		else
		{
			updatePosition(_currentMove, _simulatedNow);
		}

		if (_inputQueue.size() > 0 && _inputQueue.front()->getActionTime() <= now)
		{
			auto command = _inputQueue.front();

			if (_currentMove != nullptr && _currentMove->getCommandType() == CommandType::KnockBack)
			{
				auto knockbackMove = std::dynamic_pointer_cast<KnockbackCommand>(_currentMove);
				if (command->getActionTime() >= knockbackMove->getGoalTime())
				{
					// ok
					//fmt::print("unit[{}] knockback!!                until {}(.. {}sec), command action time is {}.\n", _unitId, knockbackMove->endTime, (knockbackMove->endTime - _simulatedNow) / 1000.0, command->getActionTime());
				}
				else
				{
					// blocked
					//fmt::print("unit[{}] knockback!! input dropping until {}(.. {}sec), command action time is {}.\n", _unitId, knockbackMove->endTime, (knockbackMove->endTime - _simulatedNow) / 1000.0, command->getActionTime());
					//fmt::print("unit[{}] now:{} simulationNow:{} endTime:{} actionTime:{}\n", _unitId, now, now - _simulatedNow, knockbackMove->endTime - _simulatedNow, command->getActionTime() - _simulatedNow);
					//fmt::print("unit[{}] CommandType:{} dropped\n", _unitId, static_cast<int>(command->getCommandType()));
					_simulatedNow = now;
					break;
				}
			}
			_inputQueue.pop_front();

			switch (command->getCommandType())
			{
			case CommandType::Move:
				{
					auto moveCommand = std::dynamic_pointer_cast<MoveCommand>(command);
					_simulatedNow = moveCommand->startTime;
					moveCommand->lastMoveCommand = _currentMove;
					_history.emplace_back(moveCommand);
					_currentMove = moveCommand;
					_isMoving = true;
				}
				break;
			case CommandType::KnockBack:
				{
					auto moveCommand = std::dynamic_pointer_cast<MoveCommand>(command);
					_simulatedNow = moveCommand->startTime;
					moveCommand->lastMoveCommand = _currentMove;
					_history.emplace_back(moveCommand);
					_currentMove = moveCommand;
					_isMoving = true;
				}
				break;
			case CommandType::Stop:
				{
					if (_currentMove == nullptr)
					{
						break;
					}

					auto stopCommand = std::dynamic_pointer_cast<StopCommand>(command);
					_simulatedNow = stopCommand->stopTime;
					stopCommand->lastMoveCommand = _currentMove;
					assert(_currentMove != nullptr);
					_history.emplace_back(stopCommand);
					_currentMove = nullptr;
					_isMoving = false;
				}
				break;
			}
		}
		else
		{
			_simulatedNow = now;
		}
	}

	for (auto& component : _components)
	{
		component.second->update(shared_from_this(), now);
	}

	while (_history.size() > 2)
	{
		if (!_history.front()->isExpired(now))
		{
			break;
		}

		//fmt::print("remove old history\n");
		_history.pop_front();
	}
}

void Unit::resetCommands()
{
	auto position = getPosition();
	_inputQueue.clear();
	_history.clear();
	setPosition(position);
}

Eigen::Vector3f Unit::getTrackbackPosition(int64_t now) const
{
	if (_history.empty())
	{
		return Eigen::Vector3f();
	}
	auto past = _history.begin();
	auto next = ++past;
	while (next != _history.end())
	{
		if ((*next)->getActionTime() > now)
		{
			break;
		}

		past = next++;
	}

	auto lastMoveCommand = std::dynamic_pointer_cast<MoveCommand>((*past));
	long lastActionTime = now;
	switch ((*past)->getCommandType())
	{
	case CommandType::Move:
		break;
	case CommandType::KnockBack:
		{
			auto knockbackCommand = std::dynamic_pointer_cast<KnockbackCommand>((*past));
			lastActionTime = std::min(lastActionTime, knockbackCommand->endTime);
		}
		break;
	case CommandType::Stop:
		{
			auto stopCommand = std::dynamic_pointer_cast<StopCommand>((*past));
			lastMoveCommand = stopCommand->lastMoveCommand.lock();
			lastActionTime = stopCommand->getActionTime();
		}
		break;
	}

	if (lastMoveCommand == nullptr)
	{
		dump(now);
	}

	auto distance = (lastMoveCommand->to - lastMoveCommand->from).norm();
	auto progress = lastMoveCommand->speed > 0 ? std::min(1.0f, (lastActionTime - lastMoveCommand->startTime) / (distance / lastMoveCommand->speed)) : 0;
	return lerp(lastMoveCommand->from, lastMoveCommand->to, progress);
}

const Eigen::Vector3f& Unit::getPosition() const
{
	return _position;
}

void Unit::setPosition(const Eigen::Vector3f& position)
{
	_history.clear();

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

	_history.emplace_back(moveCommand);
	_history.emplace_back(stopCommand);

	_currentMove.reset();
}

void Unit::dump(int64_t now) const
{
	for (auto& input : _inputQueue)
	{
		fmt::print("InputQueue: CommandType: {}, Expired: {}, ActionTime: {}\n", static_cast<int32_t>(input->getCommandType()), input->isExpired(now), input->getActionTime());
	}
	for (auto& command : _history)
	{
		fmt::print("History: CommandType: {}, Expired: {}, ActionTime: {}\n", static_cast<int32_t>(command->getCommandType()), command->isExpired(now), command->getActionTime());
	}
}
