using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torikime
{
	namespace Channel
	{
		namespace Create
		{
			public class Rpc : Torikime.IRpc
			{
                public uint ContractId => 2;
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
								case Potato.Network.Protocol.Meta.Response:
									onCreateResponse(payload);
									return true;


								case Potato.Network.Protocol.Meta.Notification:
									onCreateNotification(payload);
									return true;

								default:
									return false;
							}
						default:
							return false;
					}
				}
				public delegate void ResponseCallback(Response response);
                private Dictionary<uint, Action<Response>> responseCallbacks = new Dictionary<uint, Action<Response>>();
				private uint requestId = 0;
				public void Request(Request request, ResponseCallback callback)
				{
					RequestParcel parcel = new RequestParcel();
					parcel.RequestId = ++requestId;
					parcel.Request = request;

                    responseCallbacks.Add(parcel.RequestId, (response) => { callback(response); });
				}

                public IEnumerator RequestCoroutine(Request request, ResponseCallback callback)
                {
					RequestParcel parcel = new RequestParcel();
					parcel.RequestId = ++requestId;
					parcel.Request = request;

					bool wait = true;
                    responseCallbacks.Add(parcel.RequestId, (response) => { wait = false; callback(response); });
					while (wait)
					{
						yield return null;
					}
                }

                

				void onCreateResponse(Potato.Network.Protocol.Payload payload)
				{
                    ResponseParcel responseParcel = new ResponseParcel();
                    responseParcel.MergeFrom(new Google.Protobuf.CodedInputStream(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, payload.GetBuffer().Length - Potato.Network.Protocol.PayloadHeader.Size));

					if (responseCallbacks.TryGetValue(responseParcel.RequestId, out var callback))
					{
						callback(responseParcel.Response);
						responseCallbacks.Remove(responseParcel.RequestId);
					}
				}



				public event Action<Notification> OnNotification;
				void onCreateNotification(Potato.Network.Protocol.Payload payload)
				{
                    NotificationParcel notificationParcel = new NotificationParcel();
                    notificationParcel.MergeFrom(new Google.Protobuf.CodedInputStream(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, payload.GetBuffer().Length - Potato.Network.Protocol.PayloadHeader.Size));
                    OnNotification?.Invoke(notificationParcel.Notification);
				}

			}
		}
	}
}