#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/diagnosis_sever_sessions.pb.h"
#include "diagnosis_sever_sessions.h"

namespace torikime::diagnosis::sever_sessions
{
	Rpc::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		send(false, {});
	}

	void Rpc::Responser::send(bool success, torikime::diagnosis::sever_sessions::Response&& response)
	{
		torikime::diagnosis::sever_sessions::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		potato::net::protocol::Payload payload;
		payload.getHeader().contract_id = 2;
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
	void Rpc::onSeverSessionsRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::diagnosis::sever_sessions::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void Rpc::subscribeRequest(Rpc::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	void Rpc::deserialize(const potato::net::protocol::Payload& payload, torikime::diagnosis::sever_sessions::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onSeverSessionsRequest(payload);
			return true;

        default:
            return false;
		}
	}

}