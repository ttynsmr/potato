// Generated by the torikime.  DO NOT EDIT!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torikime
{
    namespace Example
    {
        namespace Spawn
        {
            public class Rpc : Torikime.IRpc
            {
                public ushort ContractId => 3;
                public ushort RpcId => 1;

                private Potato.Network.Session session;
                public Rpc(Potato.Network.Session session)
                {
                    this.session = session;
                }

                public bool ReceievePayload(Potato.Network.Protocol.Payload payload)
                {
                    switch (payload.Header.rpc_id)
                    {
                        case 1:
                            switch ((Potato.Network.Protocol.Meta) payload.Header.meta)
                            {

                                case Potato.Network.Protocol.Meta.Notification:
                                    onSpawnNotification(payload);
                                    return true;

                                default:
                                    return false;
                            }
                        default:
                            return false;
                    }
                }


                public event Action<Notification> OnNotification;
                void onSpawnNotification(Potato.Network.Protocol.Payload payload)
                {
                    NotificationParcel notificationParcel = new NotificationParcel();
                    notificationParcel.MergeFrom(new Google.Protobuf.CodedInputStream(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, payload.GetBuffer().Length - Potato.Network.Protocol.PayloadHeader.Size));
                    OnNotification?.Invoke(notificationParcel.Notification);
                }

            }
        }
    }
}