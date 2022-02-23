using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torikime
{
	namespace Chat
	{
		namespace SendStamp
		{
			public class Rpc : Torikime.IRpc
			{
                public uint ContractId => 1;
                public uint RpcId => 1;

                private Potato.Network.Session session;
                public Rpc(Potato.Network.Session session)
                {
                    this.session = session;
                }

				public bool ReceievePayload(Potato.Network.Protocol.Payload payload)
				{
					switch (payload.GetHeader().rpc_id)
					{
						case 1:
							switch ((Potato.Network.Protocol.Meta) payload.GetHeader().meta)
							{

								case Potato.Network.Protocol.Meta.Notification:
									onSendStampNotification(payload);
									return true;

								default:
									return false;
							}
						default:
							return false;
					}
				}


				public event Action<Notification> OnNotification;
				void onSendStampNotification(Potato.Network.Protocol.Payload payload)
				{
                    NotificationParcel notificationParcel = new NotificationParcel();
                    notificationParcel.MergeFrom(new Google.Protobuf.CodedInputStream(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, payload.GetBuffer().Length - Potato.Network.Protocol.PayloadHeader.Size));
                    OnNotification?.Invoke(notificationParcel.Notification);
				}

			}
		}
	}
}