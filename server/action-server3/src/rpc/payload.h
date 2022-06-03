#pragma once
#include <cstdint>
#include <vector>

namespace potato
{
	class PayloadHeader;
}

#include "payload_header.pb.h"

namespace potato::net::protocol
{
	struct Payload final
	{
		Payload(const potato::PayloadHeader& header);

		potato::PayloadHeader& getHeader() { return _header; }
		const potato::PayloadHeader& getHeader() const { return _header; }
		std::byte *getPayloadData() { return _buffer.data(); };
		const std::byte *getPayloadData() const { return _buffer.data(); };

		std::vector<std::byte>& getBuffer() { return _buffer; }

	private:
		potato::PayloadHeader _header;
		std::vector<std::byte> _buffer;
	};
}
