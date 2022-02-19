#include "session.h"

#include <cstdlib>
#include <iostream>
#include <memory>
#include <utility>
#include <boost/asio.hpp>

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

    void session::do_read()
    {
        auto self(shared_from_this());
        socket_.async_read_some(boost::asio::buffer(data_, max_length),
            [this, self](boost::system::error_code ec, std::size_t length)
            {
                if (!ec)
                {
                    do_write(length);
                }
            });
    }

    void session::do_write(std::size_t length)
    {
        auto self(shared_from_this());

        std::string sendbuf = std::string(data_, length);

        sendbuf = std::string("Response ----> ") + sendbuf;

        boost::asio::async_write(socket_, boost::asio::buffer(sendbuf.c_str(), sendbuf.length()),
            [this, self](boost::system::error_code ec, std::size_t /*length*/)
            {
                if (!ec)
                {
                    do_read();
                }
            });
    }
}
