#include "session.h"

#include <cstdlib>
#include <iostream>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include "Payload.h"

namespace potato::net
{
    session::session(boost::asio::ip::tcp::socket socket, SessionId sessionId)
        : _socket(std::move(socket)), _sessionId(sessionId)
    {
    }

    std::shared_ptr<session> session::start()
    {
        do_read();
        return shared_from_this();
    }

    void session::sendPayload(protocol::Payload& payload)
    {
        //std::cout << "session::sendPayload.\n";
        auto self(shared_from_this());

        _socket.async_write_some(boost::asio::buffer(payload.getBuffer()),
            [this, self](boost::system::error_code /*ec*/, std::size_t /*length*/) {});
    }

    void session::do_read()
    {
        //std::cout << "session::do_read.\n";
        auto self(shared_from_this());
        //boost::asio::streambuf::mutable_buffers_type mutableBuffer = receive_buffer_.prepare(max_length);
        _socket.async_read_some(boost::asio::buffer(data_),
            [this, self](boost::system::error_code ec, std::size_t length)
            {
                if (!ec)
                {
                    //std::cout << length << "bytes received.\n";
                    //std::cout << receive_buffer_.size() << "received buffer size.\n";

                    if (length > sizeof(protocol::PayloadHeader))
                    {
                        const protocol::PayloadHeader* header = reinterpret_cast<const protocol::PayloadHeader*>(&data_[0]);
                        const auto percelSize = sizeof(protocol::PayloadHeader) + header->payloadSize;
                        //std::cout << percelSize << "bytes percelSize needed.\n";
                        if (length >= percelSize)
                        {
                            //std::cout << percelSize << "percelSize received.\n";

                            protocol::Payload payload;
                            payload.setBufferSize(header->payloadSize);
                            boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()), boost::asio::buffer(data_));
                            _receive_buffer.consume(percelSize);

                            _receivePayloadDelegate(payload);
                        }
                    }

                    do_read();
                }
                else
                {
                    // disconnect
                    _disconnectDelegate(_sessionId);
                }
            });
    }
}
