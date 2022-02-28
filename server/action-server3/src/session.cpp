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

	void session::sendPayload(std::shared_ptr<potato::net::protocol::Payload> payload)
	{
		auto self(shared_from_this());

		_socket.async_write_some(boost::asio::buffer(payload->getBuffer()),
			[this, self, payload](boost::system::error_code /*ec*/, std::size_t /*length*/) {});
	}

	void session::readHeader()
	{
		auto self(shared_from_this());

		const protocol::PayloadHeader header = *boost::asio::buffer_cast<const protocol::PayloadHeader*>(_receive_buffer.data());
		auto headerSize = sizeof(protocol::PayloadHeader);
		_receive_buffer.consume(headerSize);

		const auto percelSize = static_cast<int32_t>(sizeof(protocol::PayloadHeader) + header.payloadSize);
		auto needsPercelSize = std::max(0, percelSize - static_cast<int32_t>(_receive_buffer.size()));
		if (needsPercelSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needsPercelSize),
				[this, self, header](boost::system::error_code ec, std::size_t /*length*/)
				{
					if (!ec)
					{
						readPercel(header);
						do_read();
					}
					else
					{
						// disconnect
						_disconnectDelegate(_sessionId);
					}
				});
		}
		else
		{
			readPercel(header);
			do_read();
		}
	}

	void session::readPercel(const protocol::PayloadHeader& header)
	{
		const auto percelSize = header.payloadSize;
		protocol::Payload payload;
		payload.setBufferSize(header.payloadSize);
		payload.getHeader() = header;
		boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()) + sizeof(protocol::PayloadHeader), boost::asio::buffer(_receive_buffer.data()));
		_receive_buffer.consume(percelSize);

		_receivePayloadDelegate(payload);
	}

	void session::do_read()
	{
		auto self(shared_from_this());

		auto needHeaderSize = std::max(0, static_cast<int32_t>(sizeof(protocol::PayloadHeader)) - static_cast<int32_t>(_receive_buffer.size()));
		if (needHeaderSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needHeaderSize),
				[this, self](boost::system::error_code ec, std::size_t /*length*/)
				{
					if (!ec)
					{
						_lastReceivedTick = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
						readHeader();
					}
					else
					{
						// disconnect
						_disconnectDelegate(_sessionId);
					}
				});
		}
		else
		{
			readHeader();
		}
	}
}
