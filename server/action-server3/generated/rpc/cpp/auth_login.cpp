#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/auth_login.pb.h"
#include "auth_login.h"

namespace torikime::auth::login
{
	Rpc::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		send(false, {});
	}

	void Rpc::Responser::send(bool success, torikime::auth::login::Response&& response)
	{
		torikime::auth::login::ResponseParcel responseParcel;
		responseParcel.set_request_id(_requestId);
		responseParcel.set_allocated_response(&response);
		responseParcel.set_success(success);

		potato::net::protocol::Payload payload;
		payload.getHeader().contract_id = 0;
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
	void Rpc::onLoginRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::auth::login::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void Rpc::subscribeRequest(Rpc::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	void Rpc::deserialize(const potato::net::protocol::Payload& payload, torikime::auth::login::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onLoginRequest(payload);
			return true;

        default:
            return false;
		}
	}

}