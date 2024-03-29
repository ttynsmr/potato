#pragma once

#include <memory>
#include <iostream>
#include <random>
#include <boost/signals2/signal.hpp>

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
	class AreaRegistry;
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

namespace potato::unit::spawn_ready
{
	class Response;
}

enum QueueProcessStage : int
{
	OnUpdate,
	OnSleep,
};

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
	GameServiceProvider();

	bool isRunning() override;

	void initialize();

	void generateNPCs();

	void onUnregisterUser(std::shared_ptr<potato::User> user);

	void onAccepted(std::shared_ptr<potato::net::Session> session);

	void subscribeRequestDiagnosisServerSessions();
	void subscribeRequestDiagnosisPingPong();
	void subscribeRequestDiagnosisCommand();
	void subscribeRequestAuthLogin();
	void subscribeRequestChatSendMessage();
	void subscribeRequestAreaTransport();
	void subscribeRequestAreaConstitutedData();
	void subscribrRequestUnitSpawnReady();
	void subscribeRequestUnitMove();
	void subscribeRequestUnitStop();
	void subscribeRequestBattleSkillCast();

	void onSessionStarted(std::shared_ptr<potato::net::Session> session);

	void onDisconnected(std::shared_ptr<potato::net::Session> session);

	void sendSystemMessage(const std::string& message);

	void sendAreacastSpawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> spawnUnit);
	void appendNeighborUnits(potato::unit::spawn_ready::Response& response, std::shared_ptr<Unit> spawnUnit);
	void sendAreacastDespawnUnit(potato::net::SessionId sessionId, std::shared_ptr<Unit> despaenUnit);

	void sendMove(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<MoveCommand> moveCommand);
	void sendStop(potato::net::SessionId sessionId, std::shared_ptr<Unit> unit, std::shared_ptr<StopCommand> stopCommand);

	void main();

	void start() override;

	void stop() override;

	std::default_random_engine& getRandomEngine();

	std::shared_ptr<potato::AreaRegistry> getAreaRegistry();

	using SynchronizedAction = std::function<void()>;
	void enqueueSynchronizedAction(SynchronizedAction action);

	using OnSpawnReadyRequestDelegate = std::function<void(std::shared_ptr<Unit> unit)>;
	boost::signals2::connection subscribeOnSpawnReadyRequest(OnSpawnReadyRequestDelegate onSpawnReadyRequestRequest);

	using OnTransportRequestDelegate = std::function<void(uint64_t transportId)>;
	boost::signals2::connection subscribeOnTransportRequest(OnTransportRequestDelegate onTransportRequest);

private:
	boost::signals2::signal<void(std::shared_ptr<Unit> unit)> _onSpawnReadyRequest;
	boost::signals2::signal<void(uint64_t transportId)> _onTransportRequest;
	std::shared_ptr<ServiceRegistry> _service;
	std::shared_ptr<potato::UserRegistry> _userRegistry;
	std::shared_ptr<potato::UnitRegistry> _unitRegistry;
	std::shared_ptr<potato::AreaRegistry> _areaRegistry;
	int64_t messageId = 0;
	std::weak_ptr<NetworkServiceProvider> _networkServiceProvider;
	std::shared_ptr<RpcBuilder> _rpcBuilder;
	std::atomic<bool> _running = true;
	std::thread _thread;
	eventpp::EventQueue<int, void(SynchronizedAction)> queue;
	IdLookupContainer _idMapper;
	std::random_device _randomDevice;
	std::default_random_engine _randomEngine;
	uint64_t _attackId = 0;
};
