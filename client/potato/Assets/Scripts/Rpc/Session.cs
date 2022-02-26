using System.Collections;
using System.Collections.Generic;
using Potato.Network.Protocol;
using UnityEngine;
using System.Net.Sockets;

namespace Potato
{
    namespace Network
    {
        public class Session
        {
            public Session(TcpClient client)
            {
                client.NoDelay = true;
                this.client = client;
                rpcs = Torikime.RpcBuilder.Build(this);
            }

            public void Disconnect()
            {
                rpcs.Clear();
                client.Close();
                client.Dispose();
            }

            public void SendPayload(Payload payload)
            {
                // Debug.Log("sent: " + payload.GetBuffer().Length + " bytes");
                client.GetStream().Write(payload.GetBuffer(), 0, payload.GetBuffer().Length);
            }

            public T GetRpc<T>() where T : Torikime.IRpc
            {
                return (T)rpcs.Find(x => x is T);
            }

            public IEnumerator ReceivePayload()
            {
                while (true)
                {
                    while (client.Available <= PayloadHeader.Size)
                    {
                        yield return null;
                    }

                    byte[] headerBuffer = new byte[PayloadHeader.Size];
                    client.GetStream().Read(headerBuffer, 0, PayloadHeader.Size);
                    var header = PayloadHeader.Deserialize(headerBuffer);
                    while (client.Available <= header.payloadSize)
                    {
                        yield return null;
                    }

                    Payload payload = new Payload();
                    payload.Header = header;
                    payload.SetBufferSize(header.payloadSize);
                    client.GetStream().Read(payload.GetBuffer(), PayloadHeader.Size, header.payloadSize);
                    // Debug.Log("received: " + payload.GetBuffer().Length + " bytes");
                    // Debug.Log("contract:" + header.contract_id + ", rpc:" + header.rpc_id + ", meta" + header.meta + ", size" + header.payloadSize);

                    rpcs.Find(x => x.ContractId == header.contract_id
                     && x.RpcId == header.rpc_id)?.ReceievePayload(payload);
                }
            }

            public void Update()
            {
                var position = Input.mousePosition;
            }

            private List<Torikime.IRpc> rpcs;

            private TcpClient client;
        }
    }
}
