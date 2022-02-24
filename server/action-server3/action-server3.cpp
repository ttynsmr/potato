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

#include "src/session.h"
#include "src/Payload.h"

#include "proto/message.pb.h"
#include "proto/chat_send_message.pb.h"
#include "proto/diagnosis_sever_sessions.pb.h"
#include "proto/example_update_mouse_position.pb.h"

#include "src/rpc.h"
#include "generated/rpc/cpp/chat_send_message.h"
#include "generated/rpc/cpp/diagnosis_sever_sessions.h"
#include "generated/rpc/cpp/example_update_mouse_position.h"

using boost::asio::ip::tcp;

class server
{
public:
    server(boost::asio::io_context& io_context, unsigned short port)
        : _acceptor(io_context, tcp::endpoint(tcp::v4(), port))
    {
        do_accept();
    }

    void sendTo(potato::net::SessionId _sessionId, potato::net::protocol::Payload& payload)
    {
        auto session = _sessions.find(_sessionId);
        if (session == _sessions.end())
        {
            return;
        }

        session->second->sendPayload(payload);
    }

    void sendMulticast(const std::vector<potato::net::SessionId>& sessionIds, potato::net::protocol::Payload&& payload)
    {
        for (auto _sessionId : sessionIds)
        {
            sendTo(_sessionId, payload);
        }
    }

    void sendBroadcast(potato::net::SessionId fromSessionId, potato::net::protocol::Payload&& payload)
    {
        for (auto& sessionPair : _sessions)
        {
            if (fromSessionId == sessionPair.first)
            {
                continue;
            }

            sessionPair.second->sendPayload(payload);
        }
    }

private:
    void do_accept()
    {
        _acceptor.async_accept(
            [this](boost::system::error_code ec, tcp::socket socket)
            {
                std::cout << "async_accept\n";
                if (!ec)
                {
                    auto session = std::make_shared<potato::net::session>(std::move(socket), ++_sessionId);
                    session->subscribeDisconnect([this, session](potato::net::SessionId _sessionId) {
                        auto r = std::remove_if(_rpcs.begin(), _rpcs.end(), [_sessionId](auto& rpc) { return rpc->getSession()->getSessionId() == _sessionId; });
                        _rpcs.erase(r, _rpcs.end());
                        _sessions.erase(_sessionId);
                        std::cout << "session: " << _sessionId << " disconnected. current session count is " << _sessions.size() << "\n";
                        });

                    auto chat = std::make_shared<torikime::chat::send_message::Rpc>(session);
                    std::weak_ptr<torikime::chat::send_message::Rpc> weak_chat = chat;
                    chat->subscribeRequest([this, weak_chat, session](const torikime::chat::send_message::RequestParcel& request, std::shared_ptr<torikime::chat::send_message::Rpc::Responser>& responser) {
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
                        sendBroadcast(session->getSessionId(), weak_chat.lock()->serializeNotification(notification));
                    });
                    _rpcs.emplace_back(chat);

                    auto diagnosis = std::make_shared<torikime::diagnosis::sever_sessions::Rpc>(session);
                    diagnosis->subscribeRequest([this](const torikime::diagnosis::sever_sessions::RequestParcel&, std::shared_ptr<torikime::diagnosis::sever_sessions::Rpc::Responser>& responser) {
                        torikime::diagnosis::sever_sessions::Response response;
                        response.set_session_count(_sessions.size());
                        responser->send(true, std::move(response));
                    });
                    _rpcs.emplace_back(diagnosis);

                    auto example = std::make_shared<torikime::example::update_mouse_position::Rpc>(session);
                    std::weak_ptr<torikime::example::update_mouse_position::Rpc> weak_example = example;
                    example->subscribeRequest([this, weak_example, session](const torikime::example::update_mouse_position::RequestParcel& request, std::shared_ptr<torikime::example::update_mouse_position::Rpc::Responser>& responser) {
                        auto position = request.request().position();
                        torikime::example::update_mouse_position::Response response;
                        responser->send(true, std::move(response));

                        torikime::example::update_mouse_position::Notification notification;
                        notification.set_allocated_position(&position);
                        sendBroadcast(session->getSessionId(), weak_example.lock()->serializeNotification(notification));
                        notification.release_position();
                        });
                    _rpcs.emplace_back(example);

                    session->subscribeReceivePayload([this, session](const potato::net::protocol::Payload& payload) {
                        auto rpc = std::find_if(_rpcs.begin(), _rpcs.end(), [session, &payload](auto& rpc) {
                            return rpc->getSession()->getSessionId() == session->getSessionId() && rpc->getContractId() == payload.getHeader().contract_id;
                            });
                        if (rpc != _rpcs.end())
                        {
                            (*rpc)->receievePayload(payload);
                        }
                    });

                    _sessions.emplace(session->getSessionId(), session);
                    session->start();
                    std::cout << "session: " << _sessionId << " accepted. current session count is " << _sessions.size() << "\n";
                }

                do_accept();
            });
    }

    std::atomic_int _sessionId = 0;
    std::unordered_map<potato::net::SessionId, std::shared_ptr<potato::net::session>> _sessions;
    std::vector<std::shared_ptr<torikime::RpcInterface>> _rpcs;
    tcp::acceptor _acceptor;
};

class Service;
class IServiceProvider
{
public:
    IServiceProvider() {}
    virtual ~IServiceProvider() {}

    virtual bool isRunning() = 0;
    virtual void start() = 0;
    virtual void stop() = 0;
};

class NetworkServiceProvider : public IServiceProvider, public std::enable_shared_from_this<NetworkServiceProvider>
{
public:
    NetworkServiceProvider() {}
    NetworkServiceProvider(std::shared_ptr<Service> service) : _service(service)
    {
    }

    bool isRunning() override { return true; }
    void start() override
    {
        _thread = std::thread([this]() {
            server s(_io_context, _port);

            std::cout << "action server bootup\n";
            _io_context.run();
            });
    }
    void stop() override {
        _io_context.stop();
    }

    void setup(uint16_t port)
    {
        _port = port;
    }

private:
    std::shared_ptr<Service> _service;
    std::thread _thread;
    boost::asio::io_context _io_context;
    uint16_t _port = 0;
};

class GameServiceProvider : public IServiceProvider, public std::enable_shared_from_this<GameServiceProvider>
{
public:
    GameServiceProvider() {}
    GameServiceProvider(std::shared_ptr<Service> service) : _service(service)
    {
    }

    bool isRunning() override { return true; }
    void start() override {
        _thread = std::thread([this]() {
            std::cout << "start game service loop\n";
            auto prev = std::chrono::high_resolution_clock::now();
            while (_running)
            {
                auto now = std::chrono::high_resolution_clock::now();
                auto spareTime = std::chrono::high_resolution_clock::now() - prev;
                prev = now;
                std::this_thread::sleep_for(std::chrono::milliseconds(std::max(0L, 100 - std::chrono::duration_cast<std::chrono::microseconds>(spareTime).count())));
            }
            std::cout << "end game service loop\n";
            });
    }

    void stop() override {
        _running = false;
        _thread.join();
    }

private:
    std::shared_ptr<Service> _service;
    std::atomic<bool> _running = true;
    std::thread _thread;
};

class Service
{
public:
    template<typename T>
    std::shared_ptr<T> registerServiceProvider(std::shared_ptr<T> serviceProvider)
    {
        _serviceProvider.push_back(serviceProvider);
        return serviceProvider;
    }

    void run()
    {
        while (running)
        {
            std::this_thread::sleep_for(std::chrono::seconds(1));
        }
    }

private:
    bool running = true;
    std::list<std::shared_ptr<IServiceProvider>> _serviceProvider;
};


int main(int argc, char* argv[])
{
    try
    {
        if (argc != 2)
        {
            std::cerr << "Usage: async_tcp_echo_server <port>\n";
            return 1;
        }


        Service service;
        auto network = service.registerServiceProvider(std::make_shared<NetworkServiceProvider>());
        auto game = service.registerServiceProvider(std::make_shared<GameServiceProvider>());

        network->setup(std::atoi(argv[1]));
        network->start();
        game->start();
        service.run();
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
