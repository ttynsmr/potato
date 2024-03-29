#pragma once

#include <memory>
#include <boost/asio.hpp>

#include "service_provider.h"
#include "session/session_types.h"

#include "rpc/payload.h"

namespace potato
{
	class Area;
}

namespace potato::net
{
	class Session;
}

namespace potato {
	class RpcInterface;
}

class ServiceRegistry;

class NetworkServiceProvider : public IServiceProvider, public std::enable_shared_from_this<NetworkServiceProvider>
{
public:
	NetworkServiceProvider(uint16_t port);

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

	using AcceptedDelegate = std::function<void(std::shared_ptr<potato::net::Session>)>;
	void setAcceptedDelegate(AcceptedDelegate callback);

	using DisconnectDelegate = std::function<void(std::shared_ptr<potato::net::Session>)>;
	void setDisconnectedDelegate(DisconnectDelegate callback);

	using SessionStartedDelegate = std::function<void(std::shared_ptr<potato::net::Session>)>;
	void setSessionStartedDelegate(SessionStartedDelegate callback);

	void registerRpc(std::shared_ptr<potato::RpcInterface> rpc);

	int32_t getConnectionCount() const;

	void visitSessions(std::function<void(std::shared_ptr<potato::net::Session>)> processor);

	void disconnectSession(const potato::net::SessionId sessionId);

	int32_t getSendCount() const { return _sendCount; }
	int32_t getReceiveCount() const { return _receiveCount; }
	void resetCounters()
	{
		_sendCount = 0;
		_receiveCount = 0;
	}

private:
	void sendToInternal(potato::net::SessionId sessionId, std::shared_ptr<potato::net::protocol::Payload> payload);
	void doAccept();

	std::thread _thread;
	boost::asio::io_context _io_context;
	boost::asio::ip::tcp::acceptor _acceptor;
	std::atomic_int _sessionIdGenerateCounter = 0;
	std::unordered_map<potato::net::SessionId, std::shared_ptr<potato::net::Session>> _sessions;
	std::vector<std::shared_ptr<potato::RpcInterface>> _rpcs;
	std::atomic<int32_t> _sendCount = 0;
	std::atomic<int32_t> _receiveCount = 0;
	AcceptedDelegate _acceptedDelegate;
	DisconnectDelegate _disconnectedDelegate;
	SessionStartedDelegate _sessionStartedDelegate;
};
