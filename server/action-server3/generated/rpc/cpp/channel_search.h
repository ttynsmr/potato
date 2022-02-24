namespace torikime::channel::search
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

namespace torikime::channel::search
{
	class Rpc final : public RpcInterface
	{
	public:
		std::uint32_t getContractId() const override { return 4; };
		std::uint32_t getRpcId() const override { return 0; };
		std::shared_ptr<potato::net::session>& getSession() override { return _session; };

		Rpc(std::shared_ptr<potato::net::session>& session);
		class Responser final
		{
		public:
			Responser(std::shared_ptr<potato::net::session>& session, std::uint32_t requestId) : _session(session), _requestId(requestId) {}
			~Responser();

			void send(bool success, torikime::channel::search::Response&& response);

		private:
			std::shared_ptr<potato::net::session> _session;
			std::uint32_t _requestId = 0;
			bool respond = false;
		};

		using RequestDelegate = std::function<void(const torikime::channel::search::RequestParcel& request, std::shared_ptr<Responser>& responser)>;
		void subscribeRequest(RequestDelegate callback);



		bool receievePayload(const potato::net::protocol::Payload& payload) override;


	private:

		static void deserialize(const potato::net::protocol::Payload& payload, torikime::channel::search::RequestParcel& outRequest);
		void onSearchRequest(const potato::net::protocol::Payload& payload);

		RequestDelegate _requestDelegate = [](const torikime::channel::search::RequestParcel&, std::shared_ptr<Responser>&) {};

		std::shared_ptr<potato::net::session> _session;

	};
}