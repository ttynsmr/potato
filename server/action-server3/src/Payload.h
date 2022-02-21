#pragma once
#include <cstdint>
#include <vector>

namespace potato::net::protocol
{
	struct __attribute__((__packed__)) PayloadHeader final
	{
		std::uint16_t payloadSize = 0;
		std::uint8_t version = 0;
		std::uint8_t meta = 0;
		std::uint16_t contract_id = 0;
		std::uint16_t rpc_id = 0;
	};

	struct Payload final
	{
		Payload() {
			buffer.resize(sizeof(PayloadHeader));
		}
		//PayloadHeader header;

		void setBufferSize(std::size_t size) {
			buffer.resize(sizeof(PayloadHeader) + size);
			getHeader().payloadSize = size;
		};

		PayloadHeader& getHeader() { return *reinterpret_cast<PayloadHeader*>(&buffer[0]); }
		const PayloadHeader& getHeader() const { return *reinterpret_cast<const PayloadHeader*>(&buffer[0]); }
		std::byte* getPayloadData() { return &buffer[sizeof(PayloadHeader)]; };
		const std::byte* getPayloadData() const { return &buffer[sizeof(PayloadHeader)]; };

		std::vector<std::byte>& getBuffer() { return buffer; }

	private:
		std::vector<std::byte> buffer;
	};
}
