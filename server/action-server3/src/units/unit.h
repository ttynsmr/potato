#pragma once

#include <list>
#include <queue>
#include <memory>
#include <Eigen/Core>

#include "proto/message.pb.h"

#include "unit_types.h"
#include "area/area_types.h"

class ICommand {
public:
	ICommand() {}
	virtual ~ICommand() {}
};

class MoveCommand : public ICommand
{
public:
	std::weak_ptr<MoveCommand> lastMoveCommand;
	long startTime;
	Eigen::Vector3f from;
	Eigen::Vector3f to;
	float speed;
	potato::UnitDirection direction;
	uint64_t moveId;
};

class StopCommand : public ICommand
{
public:
	std::weak_ptr<MoveCommand> lastMoveCommand;
	long stopTime;
	potato::UnitDirection direction;
	uint64_t moveId;
};

class Unit
{
public:
	using SessionId = int64_t;
	Unit(UnitId unitId, SessionId sessionId);

	void inputCommand(std::shared_ptr<ICommand> command);
	void update(int64_t now);

	std::shared_ptr<ICommand> getLastCommand();
	std::shared_ptr<MoveCommand> getLastMoveCommand();
	const std::shared_ptr<MoveCommand> getLastMoveCommand() const;

	UnitId getUnitId() const
	{
		return unitId;
	}

	SessionId getSessionId() const
	{
		return sessionId;
	}

	AreaId getAreaId() const
	{
		return _areaId;
	}

	void setAreaId(AreaId areaId)
	{
		_areaId = areaId;
	}

	const Eigen::Vector3f& getPosition() const
	{
		return position;
	}

	bool isMoving() const
	{
		return _isMoving;
	}

	void setLastLatency(int32_t lastLatency)
	{
		_lastLatency = lastLatency;
	}

	int32_t getLastLatency() const { return _lastLatency; }

private:
	UnitId unitId = 0;
	SessionId sessionId = 0;
	AreaId _areaId = 0;
	int64_t simulatedNow = 0;
	int32_t _lastLatency = 0;
	Eigen::Vector3f position = {};
	std::shared_ptr<MoveCommand> currentMove;
	std::queue< std::shared_ptr<ICommand>> inputQueue;
	std::list<std::shared_ptr<ICommand>> history;
	potato::UnitDirection _direction = potato::UNIT_DIRECTION_DOWN;
	bool _isMoving = false;
};
