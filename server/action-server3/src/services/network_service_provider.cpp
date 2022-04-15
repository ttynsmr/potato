#include "network_service_provider.h"

#include <iostream>
#include <boost/asio.hpp>

#include <fmt/core.h>

#include "session/session.h"

#include "rpc/rpc.h"
#include "message.pb.h"
#include "unit_despawn.pb.h"
#include "unit_despawn.h"

#include "area/area.h"

NetworkServiceProvider::NetworkServiceProvider(uint16_t port, std::shared_ptr<ServiceRegistry> service)
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
		fmt::print("action server bootup\n");
		doAccept();
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
	if (potato::net::Session::getSystemSessionId() == sessionId)
	{
		return;
	}

	boost::asio::post(_io_context.get_executor(), [this, sessionId, payload]() {
		auto session = _sessions.find(sessionId);
		if (session == _sessions.end())
		{
			return;
		}

		session->second->sendPayload(payload);
		++_sendCount;
		});
}

void NetworkServiceProvider::sendMulticast(const std::vector<potato::net::SessionId>& sessionIds, std::shared_ptr<potato::net::protocol::Payload> payload)
{
	for (auto sessionId : sessionIds)
	{
		sendTo(sessionId, payload);
	}
}

void NetworkServiceProvider::sendAreacast(const potato::net::SessionId fromSessionId, const std::shared_ptr<potato::Area> targetArea, std::shared_ptr<potato::net::protocol::Payload> payload)
{
	for (auto sessionId : targetArea->getSessionIds())
	{
		if (fromSessionId == sessionId)
		{
			continue;
		}
		sendTo(sessionId, payload);
	}
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
			++_sendCount;
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

void NetworkServiceProvider::visitSessions(std::function<void(std::shared_ptr<potato::net::Session>)> processor)
{
	for (auto& other : _sessions)
	{
		processor(other.second);
	}
}

void NetworkServiceProvider::disconnectSession(const potato::net::SessionId sessionId)
{
	auto session = _sessions.find(sessionId);
	if (session == _sessions.end())
	{
		return;
	}

	(*session).second->disconnect();
}


void NetworkServiceProvider::doAccept()
{
	_acceptor.async_accept([this](boost::system::error_code ec, boost::asio::ip::tcp::socket socket) {
		fmt::print("async_accept\n");
		socket.set_option(boost::asio::ip::tcp::no_delay(true));
		socket.set_option(boost::asio::socket_base::send_buffer_size(128 * 1024));
		if (!ec)
		{
			auto session = std::make_shared<potato::net::Session>(std::move(socket), potato::net::SessionId(++_sessionIdGenerateCounter));
			session->subscribeDisconnect([this, session](potato::net::SessionId sessionId) mutable
				{
					_disconnectedDelegate(session);
					session.reset();

					auto r = std::remove_if(_rpcs.begin(), _rpcs.end(), [sessionId](auto& rpc) { return rpc->getSession()->getSessionId() == sessionId; });
					_rpcs.erase(r, _rpcs.end());
					_sessions.erase(sessionId);
					fmt::print("session: {} disconnected. current session count is {}\n", sessionId, _sessions.size());
				});

			_acceptedDelegate(session);

			std::weak_ptr<potato::net::Session> weakSession = session;
			session->subscribeReceivePayload([this, weakSession](const potato::net::protocol::Payload& payload) {
				_receiveCount++;
				std::shared_ptr<potato::net::Session> session = weakSession.lock();
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
			fmt::print("session: {} accepted. current session count is {}\n", session->getSessionId(), _sessions.size());

			_sessionStartedDelegate(session);
		}

		doAccept();
		});
}
