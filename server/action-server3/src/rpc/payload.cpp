#include "payload.h"

namespace potato::net::protocol
{
	Payload::Payload(const potato::PayloadHeader& header) : _header(header)
	{
		_buffer.resize(header.payload_size());
	}
}
