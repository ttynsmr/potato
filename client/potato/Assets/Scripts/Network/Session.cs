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

            private List<Rpc> rpcs = RpcBuilder.Build();
        }
    }
}