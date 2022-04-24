#pragma once

#include "core/configured_eigen.h"

#include "command_types.h"

class MoveCommand : public ICommand
{
public:
	CommandType getCommandType() const override { return CommandType::Move; }

	bool isExpired(int64_t now) const override
	{
		return startTime + 10L * 1000L < now;
	};

	long getActionTime() const override
	{
		return startTime;
	}

	bool isGoaled(int64_t now) const
	{
		return getGoalTime() <= now;
	}

	virtual int64_t getGoalTime() const { return ((to - from).norm() / speed) + startTime; }

	Eigen::Vector3f getPosition(int64_t now)
	{
		auto distance = (to - from).norm();
		auto progress = std::min(1.0f, (now - startTime) / (distance / speed));
		return lerp(from, to, progress);
	};

	std::weak_ptr<MoveCommand> lastMoveCommand;
	long startTime = 0;
	Eigen::Vector3f from;
	Eigen::Vector3f to;
	float speed = 0;
	potato::UnitDirection direction = potato::UnitDirection::UNIT_DIRECTION_DOWN;
	uint64_t moveId = 0;
};
