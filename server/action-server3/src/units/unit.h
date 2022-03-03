#pragma once

#include <list>
#include <queue>
#include <memory>
#include <gml/gml.hpp>

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
	gml::vec3 from;
	gml::vec3 to;
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

	const gml::vec3& getPosition() const
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
	gml::vec3 position = {};
	std::shared_ptr<MoveCommand> currentMove;
	std::queue< std::shared_ptr<ICommand>> inputQueue;
	std::list<std::shared_ptr<ICommand>> history;
	bool _isMoving = false;
};
