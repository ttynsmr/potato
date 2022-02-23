#include "session.h"

#include <cstdlib>
#include <iostream>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include "Payload.h"

namespace potato::net
{
    session::session(boost::asio::ip::tcp::socket socket)
        : socket_(std::move(socket))
    {
    }

    std::shared_ptr<session> session::start()
    {
        do_read();
        return shared_from_this();
    }

    void session::sendPayload(const protocol::Payload& payload)
    {
        auto self(shared_from_this());

        boost::asio::async_write(socket_, boost::asio::buffer(&payload.getHeader(), sizeof(potato::net::protocol::PayloadHeader)),
            [this, self](boost::system::error_code ec, std::size_t length)
            {
                if (!ec)
                {
                    std::cout << length << "bytes sent.\n";
                }
            });
    }

    void session::do_read()
    {
        auto self(shared_from_this());
        boost::asio::streambuf::mutable_buffers_type mutableBuffer = receive_buffer_.prepare(max_length);
        socket_.async_read_some(boost::asio::buffer(mutableBuffer),
            [this, self](boost::system::error_code ec, std::size_t length)
            {
                if (!ec)
                {
                    std::cout << length << "bytes received.\n";
                    std::cout << receive_buffer_.size() << "received buffer size.\n";

                    if (receive_buffer_.size() > sizeof(protocol::PayloadHeader))
                    {
                        const protocol::PayloadHeader* header = static_cast<const protocol::PayloadHeader*>(receive_buffer_.data().data());
                        const auto percelSize = sizeof(protocol::PayloadHeader) + header->payloadSize;
                        if (receive_buffer_.size() >= percelSize)
                        {
                            protocol::Payload payload;
                            payload.setBufferSize(header->payloadSize);
                            boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()), receive_buffer_.data());
                            receive_buffer_.consume(percelSize);

                            _receivePayloadDelegate(payload);
                        }
                    }

                    do_read();
                }
            });
    }

    //void session::do_write(std::size_t length)
    //{
    //    auto self(shared_from_this());

    //    std::string sendbuf = std::string(data_, length);

    //    sendbuf = std::string("Response ----> ") + sendbuf;

    //    boost::asio::async_write(socket_, boost::asio::buffer(sendbuf.c_str(), sendbuf.length()),
    //        [this, self](boost::system::error_code ec, std::size_t /*length*/)
    //        {
    //            if (!ec)
    //            {
    //                do_read();
    //            }
    //        });
    //}
}
