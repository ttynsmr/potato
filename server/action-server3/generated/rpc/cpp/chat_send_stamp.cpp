#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/chat_send_stamp.pb.h"
#include "chat_send_stamp.h"

namespace torikime::chat::send_stamp
{


	Rpc::Rpc(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}


	potato::net::protocol::Payload Rpc::serializeNotification(torikime::chat::send_stamp::Notification& notification)
	{
		torikime::chat::send_stamp::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		potato::net::protocol::Payload payload;
		payload.setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload.getPayloadData(), payload.getHeader().payloadSize);
        return payload;
	}



	bool Rpc::receievePayload(const potato::net::protocol::Payload& payload)
	{
		switch (payload.getHeader().rpc_id)
		{
        default:
            return false;
		}
	}

}