//
// async_tcp_echo_server.cpp
// ~~~~~~~~~~~~~~~~~~~~~~~~~
//
// Copyright (c) 2003-2020 Christopher M. Kohlhoff (chris at kohlhoff dot com)
//
// Distributed under the Boost Software License, Version 1.0. (See accompanying
// file LICENSE_1_0.txt or copy at http://www.boost.org/LICENSE_1_0.txt)
//

#include <cstdlib>
#include <iostream>
#include <memory>
#include <vector>
#include <list>
#include <utility>
#include <algorithm>
#include <boost/asio.hpp>

#include "eventpp/eventqueue.h"

#include "src/session.h"
#include "src/Payload.h"

#include "proto/message.pb.h"
#include "proto/chat_send_message.pb.h"
#include "proto/diagnosis_sever_sessions.pb.h"
#include "proto/diagnosis_ping_pong.pb.h"
#include "proto/example_update_mouse_position.pb.h"
#include "proto/example_spawn.pb.h"
#include "proto/example_despawn.pb.h"
#include "proto/unit_spawn_ready.pb.h"
#include "proto/unit_spawn.pb.h"
#include "proto/unit_despawn.pb.h"
#include "proto/unit_move.pb.h"
#include "proto/unit_stop.pb.h"

#include "src/rpc.h"
#include "rpc/generated/cpp/chat_send_message.h"
#include "rpc/generated/cpp/diagnosis_sever_sessions.h"
#include "rpc/generated/cpp/diagnosis_ping_pong.h"
#include "rpc/generated/cpp/example_update_mouse_position.h"
#include "rpc/generated/cpp/example_spawn.h"
#include "rpc/generated/cpp/example_despawn.h"
#include "rpc/generated/cpp/unit_spawn_ready.h"
#include "rpc/generated/cpp/unit_spawn.h"
#include "rpc/generated/cpp/unit_despawn.h"
#include "rpc/generated/cpp/unit_move.h"
#include "rpc/generated/cpp/unit_stop.h"
#include "src/units/unit.h"

using boost::asio::ip::tcp;

enum class ServiceProviderType
{
	Network,
	Game,
};

class IServiceProvider
{
public:
	IServiceProvider() {}
	virtual ~IServiceProvider() {}

	virtual bool isRunning() = 0;
	virtual void start() = 0;
	virtual void stop() = 0;
};

class Service
{
public:
	template <typename T>
	std::shared_ptr<T> registerServiceProvider(std::shared_ptr<T> serviceProvider)
	{
		std::scoped_lock(_serviceProvidersLock);
		_serviceProviders.push_back(serviceProvider);
		return serviceProvider;
	}

	template <typename T>
	std::shared_ptr<T> findServiceProvider()
	{
		auto serviceProvider = [this] {
			std::scoped_lock(_serviceProvidersLock);
			return std::find_if(_serviceProviders.begin(), _serviceProviders.end(), [](std::shared_ptr<IServiceProvider>& s) {
				return std::dynamic_pointer_cast<T>(s); });
		}();

		if (serviceProvider == _serviceProviders.end())
		{
			return nullptr;
		}

		return std::dynamic_pointer_cast<T>(*serviceProvider);
	}

	void run()
	{
		while (running)
		{
			std::this_thread::sleep_for(std::chrono::seconds(1));
		}
	}

	using Queue = eventpp::EventQueue<ServiceProviderType, void(const std::string&, const bool)>;
	Queue& getQueue()
	{
		return queue;
	}

private:
	std::mutex _serviceProvidersLock;
	bool running = true;
	std::list<std::shared_ptr<IServiceProvider>> _serviceProviders;
	Queue queue;
};

class NetworkServiceProvider : public IServiceProvider, public std::enable_shared_from_this<NetworkServiceProvider>
{
public:
	NetworkServiceProvider(uint16_t port, std::shared_ptr<Service> service) : _acceptor(_io_context, tcp::endpoint(tcp::v4(), port)), _service(service) {}

	bool isRunning() override { return true; }
	void start() override
	{
		_thread = std::thread([this]() {
			std::cout << "action server bootup\n";
			do_accept();
			_io_context.run();
			});
	}
	void stop() override
	{
		_io_context.stop();
	}

	enum class Send
	{
		Singlecast,
		Multicast,
		Broadcast
	};

	void sendTo(potato::net::SessionId sessionId, std::shared_ptr<potato::net::protocol::Payload> payload)
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

	void sendMulticast(const std::vector<potato::net::SessionId>& sessionIds, std::shared_ptr<potato::net::protocol::Payload> payload)
	{
		boost::asio::post(_io_context.get_executor(), [this, sessionIds, payload]() {
			for (auto sessionId : sessionIds)
			{
				sendTo(sessionId, payload);
			}});
	}

	void sendBroadcast(potato::net::SessionId fromSessionId, std::shared_ptr<potato::net::protocol::Payload> payload)
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

	using AcceptedDelegate = std::function<void(std::shared_ptr<potato::net::session>)>;
	void setAcceptedDelegate(AcceptedDelegate callback)
	{
		_acceptedDelegate = callback;
	}

	using SessionStartedDelegate = std::function<void(std::shared_ptr<potato::net::session>)>;
	void setSessionStartedDelegate(SessionStartedDelegate callback)
	{
		_sessionStartedDelegate = callback;
	}
	
	void registerRpc(std::shared_ptr<torikime::RpcInterface> rpc)
	{
		_rpcs.emplace_back(rpc);
	}

	int32_t getConnectionCount() const
	{
		return _sessions.size();
	}

	void visitSessions(std::function<void(std::shared_ptr<potato::net::session>)> processor)
	{
		for (auto& other : _sessions)
		{
			processor(other.second);
		}
	}

private:
	void do_accept()
	{
		_acceptor.async_accept([this](boost::system::error_code ec, tcp::socket socket) {
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

							{
								auto session2 = session;
								auto example = std::make_shared<torikime::example::despawn::Rpc>(session2);
								torikime::example::despawn::Notification notification;
								notification.set_session_id(_sessionId);
								sendBroadcast(_sessionId, example->serializeNotification(notification));
							}
							{
								auto session2 = session;
								auto unitDespawn = std::make_shared<torikime::unit::despawn::Rpc>(session2);
								torikime::unit::despawn::Notification notification;
								notification.set_session_id(_sessionId);
								notification.set_unit_id(_sessionId);
								sendBroadcast(_sessionId, torikime::unit::despawn::Rpc::serializeNotification(notification));
							}
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

	std::thread _thread;
	boost::asio::io_context _io_context;
	eventpp::EventQueue<Send, void(std::vector<potato::net::SessionId>, std::shared_ptr<potato::net::protocol::Payload> payload)> _sendPayloadQueue;
	std::atomic_int _sessionId = 0;
	std::unordered_map<potato::net::SessionId, std::shared_ptr<potato::net::session>> _sessions;
	std::vector<std::shared_ptr<torikime::RpcInterface>> _rpcs;
	tcp::acceptor _acceptor;
	std::shared_ptr<Service> _service;
	std::atomic<int32_t> _sendCount = 0;
	std::atomic<int32_t> _receiveCount = 0;
	AcceptedDelegate _acceptedDelegate;
	SessionStartedDelegate _sessionStartedDelegate;
};

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
	GameServiceProvider() {}
	GameServiceProvider(std::shared_ptr<Service> service) : _service(service)
	{
	}

	bool isRunning() override { return true; }

	void initialize()
	{
		_service->getQueue().appendListener(ServiceProviderType::Game, [](const std::string& s, bool b) {
			std::cout << "received queue " << s << ":" << b << "\n";
			});

		_nerworkServiceProvider.lock()->setAcceptedDelegate([this](auto _) { onAccepted(_); });
		_nerworkServiceProvider.lock()->setSessionStartedDelegate([this](auto _) { onSessionStarted(_); });
	}

	void onAccepted(std::shared_ptr<potato::net::session> session)
	{
		auto chat = std::make_shared<torikime::chat::send_message::Rpc>(session);
		std::weak_ptr<torikime::chat::send_message::Rpc> weak_chat = chat;
		chat->subscribeRequest([this, weak_chat, session](const torikime::chat::send_message::RequestParcel& request, std::shared_ptr<torikime::chat::send_message::Rpc::Responser>& responser)
			{
				//std::cout << "receive RequestParcel\n";
				const auto message = request.request().message();
				torikime::chat::send_message::Response response;
				const int64_t messageId = 0;
				response.set_message_id(messageId);
				responser->send(true, std::move(response));

				torikime::chat::send_message::Notification notification;
				notification.set_message(message);
				notification.set_message_id(messageId);
				notification.set_from(std::to_string(session->getSessionId()));
				_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), weak_chat.lock()->serializeNotification(notification)); });
		_nerworkServiceProvider.lock()->registerRpc(chat);

		auto diagnosis = std::make_shared<torikime::diagnosis::sever_sessions::Rpc>(session);
		diagnosis->subscribeRequest([this](const torikime::diagnosis::sever_sessions::RequestParcel&, std::shared_ptr<torikime::diagnosis::sever_sessions::Rpc::Responser>& responser)
			{
				torikime::diagnosis::sever_sessions::Response response;
				response.set_session_count(_nerworkServiceProvider.lock()->getConnectionCount());
				responser->send(true, std::move(response)); });
		_nerworkServiceProvider.lock()->registerRpc(diagnosis);

		auto pingPong = std::make_shared<torikime::diagnosis::ping_pong::Rpc>(session);
		pingPong->subscribeRequest([session](const torikime::diagnosis::ping_pong::RequestParcel& request, std::shared_ptr<torikime::diagnosis::ping_pong::Rpc::Responser>& responser)
			{
				//std::cout << "receieve: ping" << session->getSessionId() << " request id: " << request.request_id() << "\n";
				torikime::diagnosis::ping_pong::Response response;
				response.set_receive_time(std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count());
				response.set_send_time(std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count());
				responser->send(true, std::move(response));
				//std::cout << "send: pong" << session->getSessionId() << "\n";
			});
		_nerworkServiceProvider.lock()->registerRpc(pingPong);

		auto example = std::make_shared<torikime::example::update_mouse_position::Rpc>(session);
		std::weak_ptr<torikime::example::update_mouse_position::Rpc> weak_example = example;
		example->subscribeRequest([this, weak_example, session](const torikime::example::update_mouse_position::RequestParcel& request, std::shared_ptr<torikime::example::update_mouse_position::Rpc::Responser>& responser)
			{
				auto position = request.request().position();
				torikime::example::update_mouse_position::Response response;
				responser->send(true, std::move(response));

				torikime::example::update_mouse_position::Notification notification;
				notification.set_session_id(session->getSessionId());
				notification.set_allocated_position(&position);
				_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), weak_example.lock()->serializeNotification(notification));
				notification.release_position();
			});
		_nerworkServiceProvider.lock()->registerRpc(example);

		auto unitSpawnReady = std::make_shared<torikime::unit::spawn_ready::Rpc>(session);
		unitSpawnReady->subscribeRequest([this, session](const auto& request, auto& responser)
			{
				auto unit = std::make_shared<Unit>(session->getSessionId());
				units.emplace_back(unit);

				// response
				{
					auto area_id = request.request().area_id();
					torikime::unit::spawn_ready::Response response;
					response.set_session_id(session->getSessionId());
					response.set_unit_id(session->getSessionId()); // TODO: generate unit id at unit service
					auto position = new potato::Vector3();
					position->set_x(0);
					position->set_y(0);
					position->set_z(0);
					response.set_allocated_position(new potato::Vector3());
					response.set_direction(0);
					auto individuality = new potato::Individuality();
					individuality->set_type(potato::UNIT_TYPE_PLAYER);
					response.set_allocated_individuality(individuality);
					response.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
					auto avatar = new potato::Avatar();
					avatar->set_name(std::string("hoge"));
					response.set_allocated_avatar(avatar);
					responser->send(true, std::move(response));
				}

				{
					// broadcast spawn to neighbors
					torikime::unit::spawn::Notification notification;
					notification.set_session_id(session->getSessionId());
					notification.set_unit_id(session->getSessionId()); // TODO: generate unit id at unit service
					auto position = new potato::Vector3();
					position->set_x(0);
					position->set_y(0);
					position->set_z(0);
					notification.set_allocated_position(new potato::Vector3());
					notification.set_direction(0);
					auto individuality = new potato::Individuality();
					individuality->set_type(potato::UNIT_TYPE_PLAYER);
					notification.set_allocated_individuality(individuality);
					notification.set_cause(potato::UNIT_SPAWN_CAUSE_LOGGEDIN);
					auto avatar = new potato::Avatar();
					avatar->set_name(std::string("hoge"));
					notification.set_allocated_avatar(avatar);
					_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::spawn::Rpc::serializeNotification(notification));

					// notify neighbors to spawner
					_nerworkServiceProvider.lock()->visitSessions([this, session](auto other) {
						torikime::unit::spawn::Notification notification;
						notification.set_session_id(other->getSessionId());
						auto position = new potato::Vector3();
						position->set_x(0);
						position->set_y(0);
						position->set_z(0);
						notification.set_allocated_position(new potato::Vector3());
						notification.set_direction(0);
						auto individuality = new potato::Individuality();
						individuality->set_type(potato::UNIT_TYPE_PLAYER);
						notification.set_allocated_individuality(individuality);
						notification.set_cause(potato::UNIT_SPAWN_CAUSE_ASIS);
						auto avatar = new potato::Avatar();
						avatar->set_name(std::string("hoge"));
						notification.set_allocated_avatar(avatar);
						auto payload = torikime::unit::spawn::Rpc::serializeNotification(notification);
						_nerworkServiceProvider.lock()->sendTo(session->getSessionId(), payload);
						});
				}
			});
		_nerworkServiceProvider.lock()->registerRpc(unitSpawnReady);

		auto unitMove = std::make_shared<torikime::unit::move::Rpc>(session);
		unitMove->subscribeRequest([this, session](const auto& requestParcel, auto& responser)
			{
				{
					torikime::unit::move::Response response;
					response.set_ok(true);
					responser->send(true, std::move(response));
				}

				auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
					return requestParcel.request().unit_id() == u->getUnitId();
					});
				if (unit != units.end())
				{
					auto moveCommand = std::make_shared<MoveCommand>();
					moveCommand->startTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
					const auto& from = requestParcel.request().from();
					const auto& to = requestParcel.request().to();
					moveCommand->from = { from.x(), from.y(), from.z() };
					moveCommand->to = { to.x(), to.y(), to.z() };
					moveCommand->speed = requestParcel.request().speed();
					moveCommand->direction = 0;
					moveCommand->moveId = requestParcel.request().move_id();
					(*unit)->inputCommand(moveCommand);
				}

				{
					torikime::unit::move::Notification notification;
					notification.set_unit_id(requestParcel.request().unit_id());
					notification.set_time(requestParcel.request().time());
					auto from = requestParcel.request().from();
					auto to = requestParcel.request().to();
					notification.set_allocated_from(&from);
					notification.set_allocated_to(&to);
					notification.set_speed(requestParcel.request().speed());
					notification.set_move_id(requestParcel.request().move_id());
					_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::move::Rpc::serializeNotification(notification));
					notification.release_to();
					notification.release_from();
				}
			});
		_nerworkServiceProvider.lock()->registerRpc(unitMove);

		auto unitStop = std::make_shared<torikime::unit::stop::Rpc>(session);
		unitStop->subscribeRequest([this, session](const auto& requestParcel, auto& responser)
			{
				{
					torikime::unit::stop::Response response;
					response.set_ok(true);
					responser->send(true, std::move(response));
				}

				auto unit = std::find_if(units.begin(), units.end(), [requestParcel](auto& u) {
					return requestParcel.request().unit_id() == u->getUnitId();
					});
				if (unit != units.end())
				{
					auto stopCommand = std::make_shared<StopCommand>();
					stopCommand->stopTime = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
					stopCommand->direction = 0;
					stopCommand->moveId = requestParcel.request().move_id();
					(*unit)->inputCommand(stopCommand);
				}

				{
					torikime::unit::stop::Notification notification;
					notification.set_unit_id(requestParcel.request().unit_id());
					notification.set_time(requestParcel.request().time());
					notification.set_stop_time(requestParcel.request().stop_time());
					notification.set_direction(requestParcel.request().direction());
					notification.set_move_id(requestParcel.request().move_id());
					_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), torikime::unit::stop::Rpc::serializeNotification(notification));
				}
			});
		_nerworkServiceProvider.lock()->registerRpc(unitStop);
	}

	void onSessionStarted(std::shared_ptr<potato::net::session> session)
	{
		auto example = std::make_shared<torikime::example::spawn::Rpc>(session);
		torikime::example::spawn::Notification notification;
		notification.set_session_id(session->getSessionId());
		auto position = new potato::Vector3();
		position->set_x(0);
		position->set_y(0);
		position->set_z(0);
		notification.set_allocated_position(new potato::Vector3());
		_nerworkServiceProvider.lock()->sendBroadcast(session->getSessionId(), example->serializeNotification(notification));

		_nerworkServiceProvider.lock()->visitSessions([this, example, session](auto other) {
			torikime::example::spawn::Notification notification;
			notification.set_session_id(other->getSessionId());
			auto position = new potato::Vector3();
			position->set_x(0);
			position->set_y(0);
			position->set_z(0);
			notification.set_allocated_position(new potato::Vector3());
			auto payload = example->serializeNotification(notification);
			_nerworkServiceProvider.lock()->sendTo(session->getSessionId(), payload);
			});
	}

	// TODO: move to message service
	void sendSystemMessage(const std::string& message)
	{
		torikime::chat::send_message::Notification notification;
		notification.set_message(message);
		notification.set_message_id(++messageId);
		notification.set_from("system");
		// session id 0 is system
		_nerworkServiceProvider.lock()->sendBroadcast(0, torikime::chat::send_message::Rpc::serializeNotification(notification));
	}

	void main()
	{
		auto prev = std::chrono::high_resolution_clock::now();
		while (_running)
		{
			_service->getQueue().process();

			{
				const auto now = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
				for (auto& unit : units)
				{
					unit->update(now);
				}
			}

			sendSystemMessage("hey");

			{
				auto now = std::chrono::high_resolution_clock::now();
				auto spareTime = std::chrono::high_resolution_clock::now() - prev;
				prev = now;
				std::this_thread::sleep_for(std::chrono::milliseconds(std::max(0L, 100 - std::chrono::duration_cast<std::chrono::microseconds>(spareTime).count())));
			}
		}
	}

	void start() override
	{
		_thread = std::thread([this]() {
			_nerworkServiceProvider = _service->findServiceProvider<NetworkServiceProvider>();
			initialize();
			std::cout << "start game service loop\n";
			main();
			std::cout << "end game service loop\n";
			});
	}

	void stop() override
	{
		_running = false;
		_thread.join();
	}

private:
	std::list<std::shared_ptr<Unit>> units;
	int64_t messageId = 0;
	std::weak_ptr<NetworkServiceProvider> _nerworkServiceProvider;
	std::shared_ptr<Service> _service;
	std::atomic<bool> _running = true;
	std::thread _thread;
	eventpp::EventQueue<int, void(const std::string&, const bool)> queue;
};

class SerializeServiceProvider : public IServiceProvider, public std::enable_shared_from_this<SerializeServiceProvider>
{
public:
	SerializeServiceProvider(std::shared_ptr<Service> service) : _service(service)
	{
	}

	bool isRunning()
	{
		return false;
	}

	void start() {}

	void stop() {}

private:
	std::shared_ptr<Service> _service;
};

int main(int argc, char *argv[])
{
	try
	{
		if (argc != 2)
		{
			std::cerr << "Usage: async_tcp_echo_server <port>\n";
			return 1;
		}

		std::shared_ptr<Service> service = std::make_shared<Service>();
		auto network = service->registerServiceProvider(std::make_shared<NetworkServiceProvider>(std::atoi(argv[1]), service));
		auto game = service->registerServiceProvider(std::make_shared<GameServiceProvider>(service));
		auto serialize = service->registerServiceProvider(std::make_shared<SerializeServiceProvider>(service));

		network->start();
		game->start();
		serialize->start();
		service->run();
	}
	catch (std::exception &e)
	{
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}
