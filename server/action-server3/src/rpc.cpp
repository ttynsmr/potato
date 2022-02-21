#include "rpc.h"

#include "session.h"
#include "Payload.h"
#include "proto/chat_send_message.pb.h"

namespace torikime
{
	RpcContracrChat::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		torikime::chat::send_message::Response r;
		send(false, r);
	}

	void RpcContracrChat::Responser::send(bool success, torikime::chat::send_message::Response& response)
	{
		torikime::chat::send_message::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(responseParcel.ByteSize());
		responseParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);

		_session->sendPayload(payload);

		respond = true;
	}


	RpcContracrChat::RpcContracrChat(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}

	void RpcContracrChat::onSendMessageRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::chat::send_message::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void RpcContracrChat::subscribeRequest(RpcContracrChat::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}

	void RpcContracrChat::sendNotification(torikime::chat::send_message::Notification& notification)
	{
		torikime::chat::send_message::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
		_session->sendPayload(payload);
	}

	void RpcContracrChat::deserialize(const potato::net::protocol::Payload& payload, torikime::chat::send_message::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}

	void RpcContracrChat::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onSendMessageRequest(payload);
			break;
		}
	}
}