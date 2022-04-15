#pragma once

#include <memory>
#include <iostream>
#include <random>

#include "service_registry.h"
#include "service_provider.h"
#include "session/session_types.h"

#include "user/user.h"
#include "user/user_registry.h"

#include "area/area_types.h"

namespace potato
{
	class UserRegistry;
	class UnitRegistry;
	class Area;
	class User;

	namespace net
	{
		class Session;
	}
}

class NetworkServiceProvider;
class RpcBuilder;
class Unit;
class MoveCommand;
class StopCommand;

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
	GameServiceProvider(std::shared_ptr<ServiceRegistry> service);

	bool isRunning() override;

	void initialize();

	void generateNPCs();

	void onUnregisterUser(std::shared_ptr<potato::User> user);

	void onAccepted(std::shared_ptr<potato::net::Session> session);

	void onSessionStarted(std::shared_ptr<potato::net::Session> session);

	void onDisconnected(std::shared_ptr<potato::net::Session> session);

	void sendSystemMessage(const std::string& message);

	void sendBroadcastSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit);
	void sendSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit);
	void sendDespawn(potato::net::SessionId sessionId, std::shared_ptr<Unit> despaenUnit);

	void sendMove(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<MoveCommand> moveCommand);
	void sendStop(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<StopCommand> stopCommand);

	void main();

	void start() override;

	void stop() override;

	std::default_random_engine& getRandomEngine();

	std::shared_ptr<potato::Area> getArea(potato::AreaId areaId);

private:
	GameServiceProvider() = default;
	std::shared_ptr<ServiceRegistry> _service;
	std::shared_ptr<potato::UserRegistry> _userRegistry;
	std::shared_ptr<potato::UnitRegistry> _unitRegistry;
	std::list<std::shared_ptr<potato::Area>> _areas;
	int64_t messageId = 0;
	std::weak_ptr<NetworkServiceProvider> _nerworkServiceProvider;
	std::shared_ptr<RpcBuilder> _rpcBuilder;
	std::atomic<bool> _running = true;
	std::thread _thread;
	eventpp::EventQueue<int, void(std::function<void()>)> queue;
	IdLookupContainer _idMapper;
	std::random_device _randomDevice;
	std::default_random_engine _randomEngine;
	uint64_t _attackId = 0;
};
