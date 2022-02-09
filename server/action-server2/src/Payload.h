namespace potato::net::protocol
{
	struct Payload final
	{
		std::uint16_t payloadSize = 0;
		std::uint8_t payloads[];
	};
}