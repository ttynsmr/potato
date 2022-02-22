#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/chat_send_message.pb.h"
#include "chat_send_message.h"

namespace torikime::chat::send_message
{
	RpcContract::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		torikime::chat::send_message::Response r;
		send(false, r);
	}

	void RpcContract::Responser::send(bool success, torikime::chat::send_message::Response& response)
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



	RpcContract::RpcContract(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}
	void RpcContract::onSendMessageRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::chat::send_message::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void RpcContract::subscribeRequest(RpcContract::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	potato::net::protocol::Payload RpcContract::serializeNotification(torikime::chat::send_message::Notification& notification)
	{
		torikime::chat::send_message::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
        return payload;
	}

	void RpcContract::deserialize(const potato::net::protocol::Payload& payload, torikime::chat::send_message::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool RpcContract::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onSendMessageRequest(payload);
			return true;

        default:
            return false;
		}
	}

}