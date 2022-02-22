#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/channel_create.pb.h"
#include "channel_create.h"

namespace torikime::channel::create
{
	RpcContract::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		torikime::channel::create::Response r;
		send(false, r);
	}

	void RpcContract::Responser::send(bool success, torikime::channel::create::Response& response)
	{
		torikime::channel::create::ResponseParcel responseParcel;
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
	void RpcContract::onCreateRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::channel::create::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void RpcContract::subscribeRequest(RpcContract::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	potato::net::protocol::Payload RpcContract::serializeNotification(torikime::channel::create::Notification& notification)
	{
		torikime::channel::create::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
        return payload;
	}

	void RpcContract::deserialize(const potato::net::protocol::Payload& payload, torikime::channel::create::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool RpcContract::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 1:
			onCreateRequest(payload);
			return true;

        default:
            return false;
		}
	}

}