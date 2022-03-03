#pragma once

#include <list>
#include <queue>
#include <memory>
#include <Eigen/Core>

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
	float direction;
	uint64_t moveId;
};

class StopCommand : public ICommand
{
public:
	std::weak_ptr<MoveCommand> lastMoveCommand;
	long stopTime;
	float direction;
	uint64_t moveId;
};

class Unit
{
public:
	using UnitId = int64_t;
	using SessionId = int64_t;
	Unit(UnitId unitId, SessionId sessionId);

	void inputCommand(std::shared_ptr<ICommand> command);
	void update(int64_t now);

	std::shared_ptr<ICommand> getLastCommand();
	std::shared_ptr<MoveCommand> getLastMoveCommand();

	UnitId getUnitId() const
	{
		return unitId;
	}

	SessionId getSessionId() const
	{
		return sessionId;
	}

	const Eigen::Vector3f& getPosition() const
	{
		return position;
	}

	bool isMoving() const
	{
		return _isMoving;
	}

private:
	UnitId unitId = 0;
	SessionId sessionId = 0;
	int64_t simulatedNow = 0;
	Eigen::Vector3f position = {};
	std::shared_ptr<MoveCommand> currentMove;
	std::queue< std::shared_ptr<ICommand>> inputQueue;
	std::list<std::shared_ptr<ICommand>> history;
	bool _isMoving = false;
};
