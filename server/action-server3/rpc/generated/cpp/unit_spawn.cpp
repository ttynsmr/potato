// Generated by the torikime.  DO NOT EDIT!
#include "../../../src/rpc.h"

#include "../../../src/session.h"
#include "../../../src/Payload.h"
#include "proto/unit_spawn.pb.h"
#include "unit_spawn.h"

namespace torikime::unit::spawn
{


	std::atomic<std::uint32_t> Rpc::_notificationId = 0;


	Rpc::Rpc(std::shared_ptr<potato::net::session>& session) : _session(session)
	{
	}




	std::shared_ptr<potato::net::protocol::Payload> Rpc::serializeNotification(torikime::unit::spawn::Notification& notification)
	{
		torikime::unit::spawn::NotificationParcel notificationParcel;
		notificationParcel.set_allocated_notification(&notification);
		notificationParcel.set_notification_id(++_notificationId);

		std::shared_ptr<potato::net::protocol::Payload> payload = std::make_shared<potato::net::protocol::Payload>();
		payload->getHeader().contract_id = 5;
		payload->getHeader().rpc_id = 2;
		payload->getHeader().meta = static_cast<uint8_t>(potato::net::protocol::Meta::Notification);
		payload->setBufferSize(notificationParcel.ByteSize());
		notificationParcel.SerializeToArray(payload->getPayloadData(), payload->getHeader().payloadSize);
		notificationParcel.release_notification();
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