#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/example_update_mouse_position.pb.h"
#include "example_update_mouse_position.h"

namespace torikime::example::update_mouse_position
{
	Rpc::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		send(false, {});
	}

	void Rpc::Responser::send(bool success, torikime::example::update_mouse_position::Response&& response)
	{
		torikime::example::update_mouse_position::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		potato::net::protocol::Payload payload;
		payload.getHeader().contract_id = 3;
		payload.getHeader().rpc_id = 0;
		payload.getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Response);
		payload.setBufferSize(responseParcel.ByteSize());
		responseParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
		responseParcel.release_response();

		_session->sendPayload(payload);

		respond = true;
	}



	Rpc::Rpc(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}
	void Rpc::onUpdateMousePositionRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::example::update_mouse_position::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void Rpc::subscribeRequest(Rpc::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	potato::net::protocol::Payload Rpc::serializeNotification(torikime::example::update_mouse_position::Notification& notification)
	{
		torikime::example::update_mouse_position::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.getHeader().contract_id = 3;
		payload.getHeader().rpc_id = 0;
		payload.getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Notification);
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
		notificationParcel.release_notification();
        return payload;
	}

	void Rpc::deserialize(const potato::net::protocol::Payload& payload, torikime::example::update_mouse_position::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onUpdateMousePositionRequest(payload);
			return true;

        default:
            return false;
		}
	}

}