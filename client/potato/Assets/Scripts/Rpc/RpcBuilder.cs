using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torikime
{

    public interface IRpc
    {
        ushort ContractId { get; }
        ushort RpcId { get; }
        bool ReceievePayload(Potato.Network.Protocol.Payload payload);
    }

    public static class RpcBuilder
    {
        public static List<IRpc> Build(Potato.Network.Session session)
        {
            rpcs.Add(new Auth.Login.Rpc(session));
            rpcs.Add(new Channel.Create.Rpc(session));
            rpcs.Add(new Channel.Search.Rpc(session));
            rpcs.Add(new Chat.SendMessage.Rpc(session));
            rpcs.Add(new Chat.SendStamp.Rpc(session));
            rpcs.Add(new Diagnosis.SeverSessions.Rpc(session));
            rpcs.Add(new Example.UpdateMousePosition.Rpc(session));
            return rpcs;
        }

        static List<IRpc> rpcs = new List<IRpc>();
    }
}