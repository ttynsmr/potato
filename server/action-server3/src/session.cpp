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

	void session::sendPayload(protocol::Payload &payload)
	{
		// std::cout << "session::sendPayload.\n";
		auto self(shared_from_this());

		_socket.async_write_some(boost::asio::buffer(payload.getBuffer()),
			[this, self](boost::system::error_code /*ec*/, std::size_t /*length*/) {});
	}

	void session::readHeader()
	{
		auto self(shared_from_this());

		const protocol::PayloadHeader header = *boost::asio::buffer_cast<const protocol::PayloadHeader*>(_receive_buffer.data());
		auto headerSize = sizeof(protocol::PayloadHeader);
		_receive_buffer.consume(headerSize);
		std::cout << "after consume1: " << _receive_buffer.size() << ".\n";

		const auto percelSize = static_cast<int32_t>(sizeof(protocol::PayloadHeader) + header.payloadSize);
		auto needsPercelSize = std::max(0, percelSize - static_cast<int32_t>(_receive_buffer.size()));
		if (needsPercelSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needsPercelSize),
				[this, self, header](boost::system::error_code ec, std::size_t length)
				{
					std::cout << "after read2: " << _receive_buffer.size() << ".\n";
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
		std::cout << "after readHeader: " << _receive_buffer.size() << ".\n";
	}

	void session::readPercel(const protocol::PayloadHeader& header)
	{
		const auto percelSize = header.payloadSize;
		std::cout << "percelSize: " << percelSize << ".\n";
		protocol::Payload payload;
		payload.setBufferSize(header.payloadSize);
		payload.getHeader() = header;
		boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()) + sizeof(protocol::PayloadHeader), boost::asio::buffer(_receive_buffer.data()));
		_receive_buffer.consume(percelSize);
		std::cout << "after consume2: " << _receive_buffer.size() << ".\n";

		_receivePayloadDelegate(payload);
		std::cout << "after readPercel: " << _receive_buffer.size() << ".\n";
	}

	void session::do_read()
	{
		// std::cout << "session::do_read.\n";asio::transfer_exactly(contentLength)
		auto self(shared_from_this());

		std::cout << "before read: " << _receive_buffer.size() <<  ".\n";
		auto needHeaderSize = std::max(0, static_cast<int32_t>(sizeof(protocol::PayloadHeader)) - static_cast<int32_t>(_receive_buffer.size()));
		if (needHeaderSize > 0)
		{
			boost::asio::async_read(
				_socket,
				_receive_buffer,
				boost::asio::transfer_at_least(needHeaderSize),
				[this, self](boost::system::error_code ec, std::size_t length)
				{
					std::cout << "after read1: " << _receive_buffer.size() << ".\n";
					if (!ec)
					{
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
		/*
		 boost::asio::streambuf::mutable_buffers_type mutableBuffer = _receive_buffer.prepare(1024);
		 
		_socket.async_read_some(boost::asio::buffer(mutableBuffer),
			[this, self](boost::system::error_code ec, std::size_t length)
			{
				if (!ec)
				{
					// std::cout << length << "bytes received.\n";
					// std::cout << receive_buffer_.size() << "received buffer size.\n";

					_receive_buffer.commit(length);
					if (_receive_buffer.size() > sizeof(protocol::PayloadHeader))
					{
						_receive_buffer.data.

						auto da = _receive_buffer.data();
						const protocol::PayloadHeader* header = reinterpret_cast<const protocol::PayloadHeader*>(&da);
						const auto percelSize = sizeof(protocol::PayloadHeader) + header->payloadSize;
						// std::cout << percelSize << "bytes percelSize needed.\n";
						if (_receive_buffer.size() >= percelSize)
						{
							// std::cout << percelSize << "percelSize received.\n";

							protocol::Payload payload;
							payload.setBufferSize(header->payloadSize);
							boost::asio::buffer_copy(boost::asio::buffer(payload.getBuffer()), boost::asio::buffer(da));
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
			*/
	}
}
