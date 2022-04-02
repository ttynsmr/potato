#pragma once

#include <memory>
#include <boost/asio.hpp>
#include <eventpp/eventqueue.h>

#include "service.h"
#include "service_provider.h"
#include "session/session_types.h"

#include "Payload.h"

namespace potato
{
	class Area;
}

namespace potato::net
{
	class session;
}

namespace torikime {
	class RpcInterface;
}

class NetworkServiceProvider : public IServiceProvider, public std::enable_shared_from_this<NetworkServiceProvider>
{
public:
	NetworkServiceProvider(uint16_t port, std::shared_ptr<Service> service);

	bool isRunning() override;
	void start() override;
	void stop() override;

	enum class Send
	{
		Singlecast,
		Multicast,
		Broadcast
	};

	void sendTo(potato::net::SessionId sessionId, std::shared_ptr<potato::net::protocol::Payload> payload);

	void sendMulticast(const std::vector<potato::net::SessionId>& sessionIds, std::shared_ptr<potato::net::protocol::Payload> payload);

	void sendAreacast(const potato::net::SessionId fromSessionId, const std::shared_ptr<potato::Area> targetArea, std::shared_ptr<potato::net::protocol::Payload> payload);

	void sendBroadcast(potato::net::SessionId fromSessionId, std::shared_ptr<potato::net::protocol::Payload> payload);

	using AcceptedDelegate = std::function<void(std::shared_ptr<potato::net::session>)>;
	void setAcceptedDelegate(AcceptedDelegate callback);

	using DisconnectDelegate = std::function<void(std::shared_ptr<potato::net::session>)>;
	void setDisconnectedDelegate(DisconnectDelegate callback);

	using SessionStartedDelegate = std::function<void(std::shared_ptr<potato::net::session>)>;
	void setSessionStartedDelegate(SessionStartedDelegate callback);

	void registerRpc(std::shared_ptr<torikime::RpcInterface> rpc);

	int32_t getConnectionCount() const;

	void visitSessions(std::function<void(std::shared_ptr<potato::net::session>)> processor);

	void disconnectSession(const potato::net::SessionId sessionId);

private:
	void do_accept();

	std::thread _thread;
	boost::asio::io_context _io_context;
	boost::asio::ip::tcp::acceptor _acceptor;
	eventpp::EventQueue<Send, void(std::vector<potato::net::SessionId>, std::shared_ptr<potato::net::protocol::Payload> payload)> _sendPayloadQueue;
	std::atomic_int _sessionIdGenerateCounter = 0;
	std::unordered_map<potato::net::SessionId, std::shared_ptr<potato::net::session>> _sessions;
	std::vector<std::shared_ptr<torikime::RpcInterface>> _rpcs;
	std::shared_ptr<Service> _service;
	std::atomic<int32_t> _sendCount = 0;
	std::atomic<int32_t> _receiveCount = 0;
	AcceptedDelegate _acceptedDelegate;
	DisconnectDelegate _disconnectedDelegate;
	SessionStartedDelegate _sessionStartedDelegate;
};
