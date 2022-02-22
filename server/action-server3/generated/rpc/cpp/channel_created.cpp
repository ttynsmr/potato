#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/channel_created.pb.h"
#include "channel_created.h"

namespace torikime::channel::created
{


	RpcContract::RpcContract(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}


	void RpcContract::sendNotification(torikime::channel::created::Notification& notification)
	{
		torikime::channel::created::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
		_session->sendPayload(payload);
	}



	bool RpcContract::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
        default:
            return false;
		}
	}

}