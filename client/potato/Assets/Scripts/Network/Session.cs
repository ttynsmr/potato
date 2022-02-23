using System.Collections;
using System.Collections.Generic;
using Potato.Network.Protocol;
using UnityEngine;

namespace Potato
{
    namespace Network
    {
        public class Session
        {
            public void SendPayload(Payload payload)
            {

            }

            public T GetRpc<T>() where T : Torikime.IRpc
            {
                return (T)rpcs.Find(x => x is T);
            }

            private List<Torikime.IRpc> rpcs = Torikime.RpcBuilder.Build();
        }
    }
}
