#pragma once

#include "core/configured_eigen.h"

#include "command_types.h"
#include "command_move.h"

class KnockbackCommand : public MoveCommand
{
public:
	CommandType getCommandType() const override { return CommandType::KnockBack; }

	bool isExpired(int64_t now) const override
	{
		return startTime + 10L * 1000L < now;
	};

	long getActionTime() const override
	{
		return startTime;
	}

	int64_t getGoalTime() const override
	{
		return endTime;
	}


	long endTime = 0;
};
