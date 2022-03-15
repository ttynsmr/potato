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
            var rpcs = new List<IRpc>
            {
                new Auth.Login.Rpc(session),
                new Channel.Create.Rpc(session),
                new Channel.Search.Rpc(session),
                new Chat.SendMessage.Rpc(session),
                new Chat.SendStamp.Rpc(session),
                new Battle.SkillCast.Rpc(session),
                new Diagnosis.SeverSessions.Rpc(session),
                new Diagnosis.PingPong.Rpc(session),
                new Diagnosis.Gizmo.Rpc(session),
                new Unit.SpawnReady.Rpc(session),
                new Unit.Spawn.Rpc(session),
                new Unit.Despawn.Rpc(session),
                new Unit.Move.Rpc(session),
                new Unit.Stop.Rpc(session),
                new Unit.Knockback.Rpc(session),
            };
            return rpcs;
        }

        //static List<IRpc> rpcs = new List<IRpc>();
    }
}
