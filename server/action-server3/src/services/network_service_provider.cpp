#include "network_service_provider.h"

#include <iostream>
#include <boost/asio.hpp>
//#include <boost/asio/io_context.hpp>
//#include <boost/asio/ip/tcp.hpp>
//#include <boost/asio/ip/network_v4.hpp>
//#include <boost/asio/post.hpp>

#include "session/session.h"

#include "rpc.h"
#include "proto/message.pb.h"
#include "proto/unit_despawn.pb.h"
#include "generated/cpp/unit_despawn.h"

NetworkServiceProvider::NetworkServiceProvider(uint16_t port, std::shared_ptr<Service> service)
	: _acceptor(boost::asio::ip::tcp::acceptor(_io_context, boost::asio::ip::tcp::endpoint(boost::asio::ip::tcp::v4(), port))), _service(service)
{
}

bool NetworkServiceProvider::isRunning()
{
	return true;
}

void NetworkServiceProvider::start()
{
	_thread = std::thread([this]() {
		std::cout << "action server bootup\n";
		do_accept();
		_io_context.run();
		});
}
void NetworkServiceProvider::stop()
{
	_io_context.stop();
}

enum class Send
{
	Singlecast,
	Multicast,
	Broadcast
};

void NetworkServiceProvider::sendTo(potato::net::SessionId sessionId, std::shared_ptr<potato::net::protocol::Payload> payload)
{
	boost::asio::post(_io_context.get_executor(), [this, sessionId, payload]() {
		auto session = _sessions.find(_sessionId);
		if (session == _sessions.end())
		{
			return;
		}

		session->second->sendPayload(payload);
		});
}

void NetworkServiceProvider::sendMulticast(const std::vector<potato::net::SessionId>& sessionIds, std::shared_ptr<potato::net::protocol::Payload> payload)
{
	boost::asio::post(_io_context.get_executor(), [this, sessionIds, payload]() {
		for (auto sessionId : sessionIds)
		{
			sendTo(sessionId, payload);
		}});
}

void NetworkServiceProvider::sendBroadcast(potato::net::SessionId fromSessionId, std::shared_ptr<potato::net::protocol::Payload> payload)
{
	boost::asio::post(_io_context.get_executor(), [this, fromSessionId, payload]() {
		for (auto& sessionPair : _sessions)
		{
			if (fromSessionId == sessionPair.first)
			{
				continue;
			}

			sessionPair.second->sendPayload(payload);
		}
		});
}

void NetworkServiceProvider::setAcceptedDelegate(NetworkServiceProvider::AcceptedDelegate callback)
{
	_acceptedDelegate = callback;
}

void NetworkServiceProvider::setDisconnectedDelegate(DisconnectDelegate callback)
{
	_disconnectedDelegate = callback;
}

void NetworkServiceProvider::setSessionStartedDelegate(SessionStartedDelegate callback)
{
	_sessionStartedDelegate = callback;
}

void NetworkServiceProvider::registerRpc(std::shared_ptr<torikime::RpcInterface> rpc)
{
	_rpcs.emplace_back(rpc);
}

int32_t NetworkServiceProvider::getConnectionCount() const
{
	return _sessions.size();
}

void NetworkServiceProvider::visitSessions(std::function<void(std::shared_ptr<potato::net::session>)> processor)
{
	for (auto& other : _sessions)
	{
		processor(other.second);
	}
}

void NetworkServiceProvider::do_accept()
{
	_acceptor.async_accept([this](boost::system::error_code ec, boost::asio::ip::tcp::socket socket) {
		std::cout << "async_accept\n";
		boost::asio::ip::tcp::no_delay option(true);
		socket.set_option(option);
		if (!ec)
		{
			auto session = std::make_shared<potato::net::session>(std::move(socket), ++_sessionId);
			session->subscribeDisconnect([this, session](potato::net::SessionId _sessionId)
				{
					auto r = std::remove_if(_rpcs.begin(), _rpcs.end(), [_sessionId](auto& rpc) { return rpc->getSession()->getSessionId() == _sessionId; });
					_rpcs.erase(r, _rpcs.end());
					_sessions.erase(_sessionId);
					std::cout << "session: " << _sessionId << " disconnected. current session count is " << _sessions.size() << "\n";

					_disconnectedDelegate(session);
				});

			_acceptedDelegate(session);

			session->subscribeReceivePayload([this, session](const potato::net::protocol::Payload& payload) {
				_receiveCount++;
				auto rpc = std::find_if(_rpcs.begin(), _rpcs.end(), [session, &payload](auto& rpc) {
					return rpc->getSession()->getSessionId() == session->getSessionId()
						&& rpc->getContractId() == payload.getHeader().contract_id
						&& rpc->getRpcId() == payload.getHeader().rpc_id;
					});
				if (rpc != _rpcs.end())
				{
					(*rpc)->receievePayload(payload);
				}
				});

			_sessions.emplace(session->getSessionId(), session);
			session->start();
			std::cout << "session: " << _sessionId << " accepted. current session count is " << _sessions.size() << "\n";

			_sessionStartedDelegate(session);
		}

		do_accept();
		});
}