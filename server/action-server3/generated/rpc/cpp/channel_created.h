namespace torikime::channel::created
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

namespace torikime::channel::created
{
	class RpcContract final : public RpcInterface
	{
	public:
		std::uint32_t getContractId() const override { return 2; };
		std::uint32_t getRpcId() const override { return 2; };

		RpcContract(std::shared_ptr<potato::net::session>& session);


		void sendNotification(torikime::channel::created::Notification&);

		bool receievePayload(const potato::net::protocol::Payload& payload) override;


	private:

		std::shared_ptr<potato::net::session> _session;

		std::atomic<std::uint32_t> _notificationId = 0;

	};
}