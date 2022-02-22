namespace torikime::chat::send_stamp
{
	class Notification;
	class RequestParcel;
	class Response;
	class ResponseParcel;
}
namespace potato::net::protocol
{
	struct Payload;
}
namespace potato::net
{
	class session;
}

namespace torikime::chat::send_stamp
{
	class RpcContract final : public RpcInterface
	{
	public:
		std::uint32_t getContractId() const override { return 1; };
		std::uint32_t getRpcId() const override { return 1; };

		RpcContract(std::shared_ptr<potato::net::session>& session);


		potato::net::protocol::Payload serializeNotification(torikime::chat::send_stamp::Notification&);

		bool receievePayload(const potato::net::protocol::Payload& payload) override;


	private:

		std::shared_ptr<potato::net::session> _session;

		std::atomic<std::uint32_t> _notificationId = 0;

	};
}