#pragma once

namespace potato::net::protocol
{
	struct __attribute__((__packed__)) Payload final
	{
		std::uint16_t payloadSize = 0;
		std::uint8_t payloads[];
	};
}
