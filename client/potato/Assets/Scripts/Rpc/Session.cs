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
                this.client = client;
                rpcs = Torikime.RpcBuilder.Build(this);
            }

            public void SendPayload(Payload payload)
            {
                Debug.Log("sent: " + payload.GetBuffer().Length + " bytes");
                client.GetStream().Write(payload.GetBuffer(), 0, payload.GetBuffer().Length);
            }

            public T GetRpc<T>() where T : Torikime.IRpc
            {
                return (T)rpcs.Find(x => x is T);
            }

            private List<Torikime.IRpc> rpcs;

            private TcpClient client;
        }
    }
}
