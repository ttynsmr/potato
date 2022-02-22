namespace torikime::chat::send_message
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

namespace torikime::chat::send_message
{
	class RpcContract final : public RpcInterface
	{
	public:
		std::uint32_t getContractId() const override { return 1; };
		std::uint32_t getRpcId() const override { return 0; };

		RpcContract(std::shared_ptr<potato::net::session>& session);
		class Responser final
		{
		public:
			Responser(std::shared_ptr<potato::net::session>& session, std::uint32_t requestId) : _session(session), _requestId(requestId) {}
			~Responser();

			void send(bool success, torikime::chat::send_message::Response& response);

		private:
			std::shared_ptr<potato::net::session> _session;
			std::uint32_t _requestId = 0;
			bool respond = false;
		};

		using RequestDelegate = std::function<void(const torikime::chat::send_message::RequestParcel& request, std::shared_ptr<Responser>& responser)>;
		void subscribeRequest(RequestDelegate callback);



		potato::net::protocol::Payload serializeNotification(torikime::chat::send_message::Notification&);

		bool receievePayload(const potato::net::protocol::Payload& payload) override;


	private:

		static void deserialize(const potato::net::protocol::Payload& payload, torikime::chat::send_message::RequestParcel& outRequest);
		void onSendMessageRequest(const potato::net::protocol::Payload& payload);

		RequestDelegate _requestDelegate = [](const torikime::chat::send_message::RequestParcel&, std::shared_ptr<Responser>&) {};

		std::shared_ptr<potato::net::session> _session;

		std::atomic<std::uint32_t> _notificationId = 0;

	};
}