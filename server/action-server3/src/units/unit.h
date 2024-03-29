#pragma once

#include <list>
#include <deque>
#include <memory>
#include <ostream>

#include "core/configured_eigen.h"

#include "vector3.pb.h"
#include "unit_direction.pb.h"

#include "unit_types.h"
#include "units/components/component_types.h"
#include "session/session_types.h"
#include "area/area_types.h"

#include "utility/vector_utility.h"

#include "commands/command_types.h"

class MoveCommand;
class StopCommand;

namespace potato
{
	class Area;
}

class Unit
	: public std::enable_shared_from_this<Unit>
{
public:
	Unit(UnitId unitId, potato::net::SessionId sessionId);

	void inputCommand(std::shared_ptr<ICommand> command);
	void interveneHistory(std::shared_ptr<ICommand> command);


	void onEnterArea(int64_t now, potato::AreaId areaId);
	void onLeaveArea(int64_t now, potato::AreaId areaId);
	void onSpawn(int64_t now, std::shared_ptr<potato::Area> area);
	void onDespawn(int64_t now, std::shared_ptr<potato::Area> area);
	void onConnected(int64_t now);
	void onDisconnected(int64_t now);
	void update(int64_t now);

	void resetCommands();

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

	potato::AreaId getAreaId() const
	{
		return _areaId;
	}

	void setAreaId(potato::AreaId areaId)
	{
		_areaId = areaId;
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

	const Eigen::Vector3f& getPosition() const;

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

		return std::dynamic_pointer_cast<T>(found->second);
	}

	template<typename T>
	bool hasComponent()
	{
		auto found = _components.find(typeid(T).hash_code());
		if (found == _components.end())
		{
			return false;
		}

		return true;
	}

	template<typename T>
	bool hasComponent() const
	{
		return hasComponent<T>();
	}

	template<typename T>
	void removeComponent()
	{
		_components.erase(typeid(T).hash_code());
	}

	std::string toString(int64_t now) const;
	void dump(int64_t now) const;

private:
	const UnitId _unitId = UnitId(0);
	potato::net::SessionId _sessionId = potato::net::SessionId(0);
	potato::AreaId _areaId = potato::AreaId(0);
	int64_t _simulatedNow = 0;
	int32_t _lastLatency = 0;
	Eigen::Vector3f _position = {};
	std::shared_ptr<MoveCommand> _currentMove;
	std::deque< std::shared_ptr<ICommand>> _inputQueue;
	std::list<std::shared_ptr<ICommand>> _history;
	potato::UnitDirection _direction = potato::UNIT_DIRECTION_DOWN;
	bool _isMoving = false;
	std::string _displayName;
	std::unordered_map<std::size_t, std::shared_ptr<IComponent>> _components;
};
