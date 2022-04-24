#pragma once

#include <boost/asio.hpp>
#include <queue>
#include <boost/asio.hpp>

#include "session_types.h"

namespace potato::net::protocol
{
	struct PayloadHeader;
	struct Payload;
}

namespace potato::net
{
	class Session
		: public std::enable_shared_from_this<Session>
	{
	public:
		Session(boost::asio::ip::tcp::socket socket, SessionId sessionId);

		std::shared_ptr<Session> start();
		void disconnect();

		void sendPayload(std::shared_ptr<potato::net::protocol::Payload> payload);

		using ReceivePayloadDelegate = std::function<void(const protocol::Payload& payload)>;
		void subscribeReceivePayload(ReceivePayloadDelegate callback)
		{
			_receivePayloadDelegate = callback;
		}

		using DisconnectDelegate = std::function<void(SessionId sessionId)>;
		void subscribeDisconnect(DisconnectDelegate callback)
		{
			_disconnectDelegate = callback;
		}

		SessionId getSessionId() const { return _sessionId; }

		static constexpr SessionId getSystemSessionId() { return SessionId(0); }

	private:
		void readHeader();
		void readParcel(const protocol::PayloadHeader& header);
		void doRead();

		boost::asio::ip::tcp::socket _socket;
		SessionId _sessionId;
		uint64_t _lastReceivedTick = 0;

		boost::asio::streambuf _receive_buffer;

		ReceivePayloadDelegate _receivePayloadDelegate;
		DisconnectDelegate _disconnectDelegate;
	};
}
