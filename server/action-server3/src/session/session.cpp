#include "session.h"

#include <cstdlib>
#include <iostream>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include <fmt/core.h>

#include "rpc/payload.h"

namespace potato::net
{
	Session::Session(boost::asio::ip::tcp::socket socket, SessionId sessionId)
		: _socket(std::move(socket)), _sessionId(sessionId)
	{
	}

	std::shared_ptr<Session> Session::start()
	{
		doRead();
		return shared_from_this();
	}

	void Session::disconnect()
	{
		_socket.close();
	}

	void Session::sendPayload(std::shared_ptr<potato::net::protocol::Payload> payload)
	{
		if (!_socket.is_open())
		{
			return;
		}

		auto self(shared_from_this());
		_socket.async_write_some(boost::asio::buffer(payload->getBuffer()),
			[this, self, payload](boost::system::error_code /*ec*/, std::size_t /*length*/) {
				//fmt::print("async_write_some result: ec:{}, sent length:{} payload size:{}\n", ec.value() , length, payload->getBuffer().size());
			});
	}

	void Session::readHeader()
	{
		auto self(shared_from_this());

		const protocol::PayloadHeader header = *boost::asio::buffer_cast<const protocol::PayloadHeader*>(_receive_buffer.data());
		auto headerSize = sizeof(protocol::PayloadHeader);
		_receive_buffer.consume(headerSize);

		const auto percelSize = header.payloadSize;
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
						readParcel(header);
						doRead();
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
			readParcel(header);
			doRead();
		}
	}

	void Session::readParcel(const protocol::PayloadHeader& header)
	{
		const auto percelSize = header.payloadSize;
		protocol::Payload payload;
		payload.setBufferSize(header.payloadSize);
		payload.getHeader() = header;
		boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()) + sizeof(protocol::PayloadHeader), boost::asio::buffer(_receive_buffer.data()));
		_receive_buffer.consume(percelSize);

		_receivePayloadDelegate(payload);
	}

	void Session::doRead()
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
