#include "rpc.h"

#include "session.h"
#include "Payload.h"
#include "proto/chat_send_message.pb.h"

RpcContracrChat::RpcContracrChat(std::shared_ptr<potato::net::session>& session) : _session(session)
{
}

void RpcContracrChat::onSendMessageRequest(const potato::net::protocol::Payload& payload, const RpcContracrChat::ResponseCallback& callback)
{
	torikime::chat::send_message::Request request;
	deserialize(payload, request);

	auto autoResponseCallback = std::shared_ptr<RpcContracrChat::ResponseCallback>(new std::function(callback), [](auto callback) {
		(*callback)({});
		delete callback;
	});

	_requestDelegate(request, autoResponseCallback);
}

void RpcContracrChat::subscribeRequest(RpcContracrChat::RequestDelegate callback)
{
	_requestDelegate = callback;
}

void RpcContracrChat::sendNotification(const torikime::chat::send_message::Notification& notification)
{
	potato::net::protocol::Payload payload;
	payload.payloadSize = notification.ByteSize();
	payload.buffer.resize(payload.payloadSize);
	notification.SerializeToArray(&payload.buffer[0], payload.buffer.size());
	//_session->sendPayload(payload);
}

void RpcContracrChat::deserialize(const potato::net::protocol::Payload& payload, torikime::chat::send_message::Request& outRequest)
{
	outRequest.Clear();
	outRequest.ParseFromArray(&payload.buffer[0], payload.buffer.size());
}
