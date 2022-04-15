#pragma once

#include <list>
#include <deque>
#include <memory>
#include <ostream>

#include "core/configured_eigen.h"

#include "message.pb.h"

#include "unit_types.h"
#include "units/components/component_types.h"
#include "session/session_types.h"
#include "area/area_types.h"

#include "utility/vector_utility.h"

enum class CommandType : int32_t
{
	Stop,
	Move,
	KnockBack,
};

class ICommand {
public:
	ICommand() {}
	virtual ~ICommand() {}

	virtual CommandType getCommandType() const = 0;

	virtual bool isExpired(int64_t now) const = 0;

	virtual long getActionTime() const = 0;
};

class MoveCommand : public ICommand
{
public:
	CommandType getCommandType() const override  { return CommandType::Move; }

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

class Unit
	: public std::enable_shared_from_this<Unit>
{
public:
	Unit(UnitId unitId, potato::net::SessionId sessionId);

	void inputCommand(std::shared_ptr<ICommand> command);
	void interveneHistory(std::shared_ptr<ICommand> command);


	void onSpawn(int64_t now);
	void onDespawn(int64_t now);
	void onConnected(int64_t now);
	void onDisconnected(int64_t now);
	void update(int64_t now);

	std::shared_ptr<ICommand> getLastCommand();

	std::shared_ptr<MoveCommand> getLastMoveCommand();
	const std::shared_ptr<MoveCommand> getLastMoveCommand() const;

	std::shared_ptr<StopCommand> getFirstStopCommandFromQueue();
	const std::shared_ptr<StopCommand> getFirstStopCommandFromQueue() const;


	UnitId getUnitId() const
	{
		return _unitId;
	}

	void setSessionId(potato::net::SessionId sessionId)
	{
		_sessionId = sessionId;
	}

	potato::net::SessionId getSessionId() const
	{
		return _sessionId;
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

	potato::UnitDirection getDirection() const
	{
		return _direction;
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

	Eigen::Vector3f getTrackbackPosition(int64_t now) const;

	Eigen::Vector3f getCurrentPosition() const;

	void setPosition(const Eigen::Vector3f& position);

	void setDisplayName(const std::string& displayName) { _displayName = displayName; }
	const std::string& getDisplayName() const { return _displayName; }

	template<typename T, class... Args>
	std::shared_ptr<T> addComponent(Args... args)
	{
		std::pair<std::unordered_map<std::size_t, std::shared_ptr<IComponent>>::iterator, bool> result = _components.emplace(typeid(T).hash_code(), std::make_shared<T>(args...));
		if (!result.second)
		{
			return nullptr;
		}

		std::shared_ptr<IComponent> i = result.first->second;

		return std::dynamic_pointer_cast<T>(i);
	}

	template<typename T>
	std::shared_ptr<T> getComponent()
	{
		auto found = _components.find(typeid(T).hash_code());
		if (found == _components.end())
		{
			return nullptr;
		}

		return *found;
	}

	template<typename T>
	void removeComponent()
	{
		_components.erase(typeid(T).hash_code());
	}

	void dump(int64_t now) const;

private:
	const UnitId _unitId = UnitId(0);
	potato::net::SessionId _sessionId = potato::net::SessionId(0);
	AreaId _areaId = 0;
	int64_t simulatedNow = 0;
	int32_t _lastLatency = 0;
	Eigen::Vector3f position = {};
	std::shared_ptr<MoveCommand> currentMove;
	std::deque< std::shared_ptr<ICommand>> inputQueue;
	std::list<std::shared_ptr<ICommand>> history;
	potato::UnitDirection _direction = potato::UNIT_DIRECTION_DOWN;
	bool _isMoving = false;
	std::string _displayName;
	std::unordered_map<std::size_t, std::shared_ptr<IComponent>> _components;
};
