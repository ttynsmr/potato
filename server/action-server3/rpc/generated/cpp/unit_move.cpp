// Generated by the torikime.  DO NOT EDIT!
#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/unit_move.pb.h"
#include "unit_move.h"

namespace torikime::unit::move
{
	Rpc::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		send(false, {});
	}

	void Rpc::Responser::send(bool success, torikime::unit::move::Response&& response)
	{
		torikime::unit::move::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		std::shared_ptr<potato::net::protocol::Payload> payload = std::make_shared<potato::net::protocol::Payload>();
		payload->getHeader().contract_id = 5;
		payload->getHeader().rpc_id = 0;
		payload->getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Response);
		payload->setBufferSize(responseParcel.ByteSize());
		responseParcel.SerializeToArray(payload->getPayloadData(), payload->getHeader().payloadSize);
		responseParcel.release_response();

		_session->sendPayload(payload);

		respond = true;
	}



	std::atomic<std::uint32_t> Rpc::_notificationId = 0;


	Rpc::Rpc(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}
	void Rpc::onMoveRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::unit::move::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void Rpc::subscribeRequest(Rpc::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	std::shared_ptr<potato::net::protocol::Payload> Rpc::serializeNotification(torikime::unit::move::Notification& notification)
	{
		torikime::unit::move::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		std::shared_ptr<potato::net::protocol::Payload> payload = std::make_shared<potato::net::protocol::Payload>();
		payload->getHeader().contract_id = 5;
		payload->getHeader().rpc_id = 0;
		payload->getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Notification);
		payload->setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload->getPayloadData(), payload->getHeader().payloadSize);
		notificationParcel.release_notification();
        return payload;
	}

	void Rpc::deserialize(const potato::net::protocol::Payload& payload, torikime::unit::move::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onMoveRequest(payload);
			return true;

        default:
            return false;
		}
	}

}