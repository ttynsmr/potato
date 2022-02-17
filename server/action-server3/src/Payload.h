#pragma once
#include <cstdint>

namespace potato::net::protocol
{
	struct __attribute__((__packed__)) Payload final
	{
		std::uint16_t payloadSize = 0;
		std::uint8_t version = 0;
		std::uint8_t meta = 0;

		static int getInt();
	};
}
