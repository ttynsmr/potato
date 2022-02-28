// Generated by the torikime.  DO NOT EDIT!
#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/diagnosis_ping_pong.pb.h"
#include "diagnosis_ping_pong.h"

namespace torikime::diagnosis::ping_pong
{
	Rpc::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		send(false, {});
	}

	void Rpc::Responser::send(bool success, torikime::diagnosis::ping_pong::Response&& response)
	{
		torikime::diagnosis::ping_pong::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		std::shared_ptr<potato::net::protocol::Payload> payload = std::make_shared<potato::net::protocol::Payload>();
		payload->getHeader().contract_id = 2;
		payload->getHeader().rpc_id = 1;
		payload->getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Response);
		payload->setBufferSize(responseParcel.ByteSize());
		responseParcel.SerializeToArray(payload->getPayloadData(), payload->getHeader().payloadSize);
		responseParcel.release_response();

		_session->sendPayload(payload);

		respond = true;
	}




	Rpc::Rpc(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}
	void Rpc::onPingPongRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::diagnosis::ping_pong::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void Rpc::subscribeRequest(Rpc::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	void Rpc::deserialize(const potato::net::protocol::Payload& payload, torikime::diagnosis::ping_pong::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 1:
			onPingPongRequest(payload);
			return true;

        default:
            return false;
		}
	}

}