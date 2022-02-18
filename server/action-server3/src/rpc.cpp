#include <list>
#include <functional>
#include "Payload.h"

#include "proto/chat_send_message.pb.h"

class RpcContracrChat
{
public:
	void SendMessage(const torikime::chat::send_message::Request& request, std::function<void(const torikime::chat::send_message::Response&)> callback)
	{
	}

	using Notification = std::function<void(const torikime::chat::send_message::Notification&)>;

private:
	std::list<Notification> _notifications;
};