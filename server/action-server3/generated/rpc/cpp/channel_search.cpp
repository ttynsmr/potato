#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/channel_search.pb.h"
#include "channel_search.h"

namespace torikime::channel::search
{
	RpcContract::Responser::~Responser()
	{
		if (respond)
		{
			return;
		}

		torikime::channel::search::Response r;
		send(false, r);
	}

	void RpcContract::Responser::send(bool success, torikime::channel::search::Response& response)
	{
		torikime::channel::search::ResponseParcel responseParcel;
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
	void RpcContract::onSearchRequest(const potato::net::protocol::Payload& payload)
	{
		torikime::channel::search::RequestParcel requestParcel;
		deserialize(payload, requestParcel);

		auto responser = std::make_shared<Responser>(_session, requestParcel.request_id());
		_requestDelegate(requestParcel, responser);
	}

	void RpcContract::subscribeRequest(RpcContract::RequestDelegate callback)
	{
		_requestDelegate = callback;
	}



	void RpcContract::deserialize(const potato::net::protocol::Payload& payload, torikime::channel::search::RequestParcel& outRequest)
	{
		outRequest.Clear();
		outRequest.ParseFromArray(payload.getPayloadData(), payload.getHeader().payloadSize);
	}



	bool RpcContract::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
		case 0:
			onSearchRequest(payload);
			return true;

        default:
            return false;
		}
	}

}