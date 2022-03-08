#pragma once

#include <memory>
#include <iostream>
#include "service.h"
#include "service_provider.h"

#define forward_declaration(name) \
namespace name \
{ \
	class Rpc; \
	class Responser; \
	class RequestParcel; \
	class Responser; \
	class Notification; \
} \
//

forward_declaration(torikime::chat::send_message)
forward_declaration(torikime::diagnosis::sever_sessions)
forward_declaration(torikime::diagnosis::ping_pong)
forward_declaration(torikime::unit::spawn_ready)
forward_declaration(torikime::unit::spawn)
forward_declaration(torikime::unit::despawn)

namespace potato::net
{
	class session;
}

class NetworkServiceProvider;
class Unit;

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
	GameServiceProvider();
	GameServiceProvider(std::shared_ptr<Service> service);

	bool isRunning();

	void initialize();

	void onAccepted(std::shared_ptr<potato::net::session> session);

	void onSessionStarted(std::shared_ptr<potato::net::session> session);

	void onDisconnected(std::shared_ptr<potato::net::session> session);

	void sendSystemMessage(const std::string& message);

	void main();

	void start() override;

	void stop() override;

private:
	std::list<std::shared_ptr<Unit>> units;
	int64_t messageId = 0;
	std::weak_ptr<NetworkServiceProvider> _nerworkServiceProvider;
	std::shared_ptr<Service> _service;
	std::atomic<bool> _running = true;
	std::thread _thread;
	eventpp::EventQueue<int, void(const std::string&, const bool)> queue;
};
