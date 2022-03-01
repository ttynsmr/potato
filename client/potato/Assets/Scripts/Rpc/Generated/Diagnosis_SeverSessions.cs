// Generated by the torikime.  DO NOT EDIT!
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torikime
{
    namespace Diagnosis
    {
        namespace SeverSessions
        {
            public class Rpc : Torikime.IRpc
            {
                public static ushort StaticContractId = 2;
                public static ushort StaticRpcId = 0;

                public ushort ContractId => StaticContractId;
                public ushort RpcId => StaticRpcId;

                private Potato.Network.Session session;
                public Rpc(Potato.Network.Session session)
                {
                    this.session = session;
                }

                public bool ReceievePayload(Potato.Network.Protocol.Payload payload)
                {
                    switch (payload.Header.rpc_id)
                    {
                        case 0:
                            switch ((Potato.Network.Protocol.Meta) payload.Header.meta)
                            {
                                case Potato.Network.Protocol.Meta.Response:
                                    onSeverSessionsResponse(payload);
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

                    Potato.Network.Protocol.Payload payload = new Potato.Network.Protocol.Payload();
                    using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                    using (Google.Protobuf.CodedOutputStream output = new Google.Protobuf.CodedOutputStream(ms))
                    {
                        parcel.WriteTo(output);
                        output.Flush();
                        payload.SetBufferSize((int)ms.Length);
                        payload.Header.contract_id = ContractId;
                        payload.Header.rpc_id = RpcId;
                        payload.Header.SerializeTo(payload.GetBuffer());
                        ms.Position = 0;
                        ms.Read(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, (int)ms.Length);
                        session.SendPayload(payload);
                    }
                }

                public IEnumerator RequestCoroutine(Request request, ResponseCallback callback)
                {
                    bool wait = true;
                    Request(request, (response) => {
                        wait = false;
                        callback(response);
                    });

                    while (wait)
                    {
                        yield return null;
                    }
                }

                

                void onSeverSessionsResponse(Potato.Network.Protocol.Payload payload)
                {
                    ResponseParcel responseParcel = new ResponseParcel();
                    responseParcel.MergeFrom(new Google.Protobuf.CodedInputStream(payload.GetBuffer(), Potato.Network.Protocol.PayloadHeader.Size, payload.GetBuffer().Length - Potato.Network.Protocol.PayloadHeader.Size));

                    if (responseCallbacks.TryGetValue(responseParcel.RequestId, out var callback))
                    {
                        callback(responseParcel.Response);
                        responseCallbacks.Remove(responseParcel.RequestId);
                    }
                }



            }
        }
    }
}