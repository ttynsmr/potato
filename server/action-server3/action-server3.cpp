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
#include <utility>
#include <boost/asio.hpp>

#include "src/session.h"
#include "src/Payload.h"

#include "proto/message.pb.h"
#include "proto/chat_send_message.pb.h"

#include "src/rpc.h"
#include "generated/rpc/cpp/chat_send_message.h"

using boost::asio::ip::tcp;

// payload format
// +---------------+----------------+
// |     2bytes    |     n bytes    |
// +---------------+----------------+
// | payload bytes | payload binary |
// +---------------+----------------+

class server
{
public:
    using SessionId = std::int32_t;

    server(boost::asio::io_context& io_context, unsigned short port)
        : acceptor_(io_context, tcp::endpoint(tcp::v4(), port))
    {
        do_accept();
    }

    void sendTo(SessionId sessionId, const potato::net::protocol::Payload& payload)
    {
        auto session = _sessions.find(sessionId);
        if (session == _sessions.end())
        {
            return;
        }

        session->second->sendPayload(payload);
    }

    void sendMulticast(const std::vector<SessionId>& sessionIds, const potato::net::protocol::Payload& payload)
    {
        for (auto sessionId : sessionIds)
        {
            sendTo(sessionId, payload);
        }
    }

    void sendBroadcast(const potato::net::protocol::Payload& payload)
    {
        for (auto& sessionPair : _sessions)
        {
            sessionPair.second->sendPayload(payload);
        }
    }

private:
    void do_accept()
    {
        acceptor_.async_accept(
            [this](boost::system::error_code ec, tcp::socket socket)
            {
                std::cout << "async_accept\n";
                if (!ec)
                {
                    auto session = std::make_shared<potato::net::session>(std::move(socket))->start();
                    auto chat = std::make_shared<torikime::chat::send_message::Rpc>(session);
                    _chat = chat;
                    _rpcs.emplace_back(chat);
                    _chat.lock()->subscribeRequest([this](const torikime::chat::send_message::RequestParcel& request, std::shared_ptr<torikime::chat::send_message::Rpc::Responser>& responser) {
                        std::cout << "receive RequestParcel\n";
                        auto message = request.request().message();
                        torikime::chat::send_message::Response response;
                        response.set_message_id(0);
                        responser->send(true, response);

                        torikime::chat::send_message::Notification notification;
                        notification.set_message(message);
                        notification.set_message_id(response.message_id());
                        notification.set_from(std::to_string(sessionId));
                        sendBroadcast(_chat.lock()->serializeNotification(notification));
                    });

                    session->subscribeReceivePayload([this](const potato::net::protocol::Payload& payload) {
                        for (auto& rpc : _rpcs)
                        {
                            if (rpc->getContractId() != payload.getHeader().contract_id)
                            {
                                continue;
                            }

                            rpc->receievePayload(payload);
                        }
                    });

                    _sessions.emplace(++sessionId, session);
                }

                do_accept();
            });
    }

    std::atomic_int sessionId = 0;
    std::unordered_map<SessionId, std::shared_ptr<potato::net::session>> _sessions;
    std::vector<std::shared_ptr<torikime::RpcInterface>> _rpcs;
    std::weak_ptr<torikime::chat::send_message::Rpc> _chat;
    tcp::acceptor acceptor_;
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

        boost::asio::io_context io_context;

        server s(io_context, std::atoi(argv[1]));

        io_context.run();
    }
    catch (std::exception& e)
    {
        std::cerr << "Exception: " << e.what() << "\n";
    }

    return 0;
}
