#pragma once

#include "core/configured_eigen.h"

#include "command_types.h"

class StopCommand : public ICommand
{
public:
	CommandType getCommandType() const override { return CommandType::Stop; }

	bool isExpired(int64_t now) const override
	{
		return stopTime + 10L * 1000L < now || lastMoveCommand.expired();
	};

	long getActionTime() const override
	{
		return stopTime;
	}

	std::weak_ptr<MoveCommand> lastMoveCommand;
	long stopTime = 0;
	potato::UnitDirection direction = potato::UnitDirection::UNIT_DIRECTION_DOWN;
	uint64_t moveId = 0;
};
