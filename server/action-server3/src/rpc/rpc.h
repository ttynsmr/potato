#include <memory>

namespace potato::net::protocol
{
	struct Payload;
}
namespace potato::net
{
	class Session;
}

namespace potato
{
	class RpcInterface
	{
	public:
		RpcInterface() {}
		virtual ~RpcInterface() {}

		virtual std::uint32_t getContractId() const = 0;
		virtual std::uint32_t getRpcId() const = 0;
		virtual std::shared_ptr<potato::net::Session> getSession() = 0;

		virtual bool receievePayload(const potato::net::protocol::Payload& payload) = 0;
	};
}
