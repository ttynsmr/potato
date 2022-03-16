#pragma once

#include <memory>
#include <iostream>
#include "service.h"
#include "service_provider.h"

namespace potato
{
	class UnitRegistory;
	class Area;

	namespace net
	{
		class session;
	}
}

class NetworkServiceProvider;
class Unit;

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
	GameServiceProvider(std::shared_ptr<Service> service);

	bool isRunning() override;

	void initialize();

	void onAccepted(std::shared_ptr<potato::net::session> session);

	void onSessionStarted(std::shared_ptr<potato::net::session> session);

	void onDisconnected(std::shared_ptr<potato::net::session> session);

	void sendSystemMessage(const std::string& message);

	void main();

	void start() override;

	void stop() override;

private:
	GameServiceProvider() = default;
	std::shared_ptr<Service> _service;
	std::shared_ptr<potato::UnitRegistory> _unitRegistory;
	std::list<std::shared_ptr<potato::Area>> _areas;
	int64_t messageId = 0;
	std::weak_ptr<NetworkServiceProvider> _nerworkServiceProvider;
	std::atomic<bool> _running = true;
	std::thread _thread;
	eventpp::EventQueue<int, void(std::function<void()>)> queue;

	uint64_t _attackId = 0;
};
