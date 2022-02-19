#include <unordered_map>
#include <functional>
#include <memory>

namespace torikime::chat::send_message
{
	class Notification;
	class Request;
	class Response;
}
namespace potato::net::protocol
{
	struct Payload;
}
namespace potato::net
{
	class session;
}

class RpcContracrChat
{
public:
	RpcContracrChat(std::shared_ptr<potato::net::session>& session);

	using ResponseCallback = std::function<void(const torikime::chat::send_message::Response&)>;
	using RequestDelegate = std::function<void(const torikime::chat::send_message::Request& request, const std::shared_ptr<ResponseCallback>& callback)>;

	void subscribeRequest(RequestDelegate callback);

	using Notification = std::function<void(const torikime::chat::send_message::Notification&)>;

	void sendNotification(const torikime::chat::send_message::Notification&);

private:
	static void deserialize(const potato::net::protocol::Payload& payload, torikime::chat::send_message::Request& outRequest);
	void onSendMessageRequest(const potato::net::protocol::Payload& payload, const ResponseCallback& callback);

	RequestDelegate _requestDelegate = [](const torikime::chat::send_message::Request& request, const std::shared_ptr<ResponseCallback>& callback) {};
	std::shared_ptr<potato::net::session>& _session;
};