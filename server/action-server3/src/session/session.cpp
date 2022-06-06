#include "session.h"

#include <cstdlib>
#include <iostream>
#include <memory>
#include <utility>
#include <boost/asio.hpp>
#include <fmt/core.h>

#include "rpc/payload.h"
#include "payload_header.pb.h"

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
		uint16_t payloadHeaderSize = payload->getHeader().ByteSize();
		std::vector<std::byte> singleBuffer(sizeof(uint16_t) + payloadHeaderSize + payload->getBuffer().size());
		*reinterpret_cast<uint16_t*>(singleBuffer.data()) = payloadHeaderSize;
		payload->getHeader().SerializeToArray(&singleBuffer.data()[sizeof(uint16_t)], payloadHeaderSize);
		std::memcpy(&singleBuffer.data()[sizeof(uint16_t) + payloadHeaderSize], payload->getBuffer().data(), payload->getBuffer().size());
		
		_socket.async_write_some(boost::asio::buffer(singleBuffer),
			[this, self, payload](boost::system::error_code /*ec*/, std::size_t /*length*/) { });
	}

	void Session::readPayloadHeaderSize(uint16_t payloadHeaderSize)
	{
		auto self(shared_from_this());

		auto needsHeaderSize = std::max(0, payloadHeaderSize - static_cast<int32_t>(_receive_buffer.size()));
		if (needsHeaderSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needsHeaderSize),
				[this, self, payloadHeaderSize](boost::system::error_code ec, std::size_t /*length*/)
				{
					if (!ec)
					{
						readHeader(payloadHeaderSize);
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
			readHeader(payloadHeaderSize);
		}
	}

	void Session::readHeader(int32_t payloadHeaderSize)
	{
		potato::PayloadHeader header;
		header.ParseFromArray(_receive_buffer.data().data(), payloadHeaderSize);
		_receive_buffer.consume(payloadHeaderSize);

		readParcel(std::move(header));
	}

	void Session::readParcel(potato::PayloadHeader&& header)
	{
		auto self(shared_from_this());

		auto needsPercelSize = std::max(0, header.payload_size() - static_cast<int32_t>(_receive_buffer.size()));
		if (needsPercelSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needsPercelSize),
				[this, self, header = std::move(header)](boost::system::error_code ec, std::size_t /*length*/)
				{
					if (!ec)
					{
						const auto percelSize = header.payload_size();
						protocol::Payload payload(header);
						boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()), boost::asio::buffer(_receive_buffer.data()));
						_receive_buffer.consume(percelSize);

						_receivePayloadDelegate(payload);

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
			const auto percelSize = header.payload_size();
			protocol::Payload payload(header);
			boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()), boost::asio::buffer(_receive_buffer.data()));
			_receive_buffer.consume(percelSize);

			_receivePayloadDelegate(payload);

			doRead();
		}
	}

	void Session::doRead()
	{
		auto self(shared_from_this());

		auto needFixHeaderSize = std::max(0, static_cast<int32_t>(sizeof(uint16_t)) - static_cast<int32_t>(_receive_buffer.size()));
		if (needFixHeaderSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needFixHeaderSize),
				[this, self](boost::system::error_code ec, std::size_t /*length*/)
				{
					if (!ec)
					{
						const uint16_t payloadHeaderSize = *static_cast<const uint16_t*>(_receive_buffer.data().data());
						_receive_buffer.consume(sizeof(uint16_t));

						_lastReceivedTick = std::chrono::duration_cast<std::chrono::milliseconds>(std::chrono::system_clock::now().time_since_epoch()).count();
						readPayloadHeaderSize(payloadHeaderSize);
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
			const uint16_t payloadHeaderSize = *static_cast<const uint16_t*>(_receive_buffer.data().data());
			_receive_buffer.consume(sizeof(uint16_t));

			readPayloadHeaderSize(payloadHeaderSize);
		}
	}
}
