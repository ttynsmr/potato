using System.Collections.Generic;

namespace Potato
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
                new Area.Transport.Rpc(session),
                new Area.ConstitutedData.Rpc(session),
                new Auth.Login.Rpc(session),
                new Channel.Create.Rpc(session),
                new Channel.Search.Rpc(session),
                new Chat.SendMessage.Rpc(session),
                new Chat.SendStamp.Rpc(session),
                new Battle.SkillCast.Rpc(session),
                new Battle.SyncParameters.Rpc(session),
                new Diagnosis.SeverSessions.Rpc(session),
                new Diagnosis.PingPong.Rpc(session),
                new Diagnosis.Command.Rpc(session),
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
    }

    public static class RpcHolder
    {
        public static List<IRpc> Rpcs { get; set; }

        public static void Clear()
        {
            Rpcs.Clear();
        }

        public static T GetRpc<T>() where T : IRpc
        {
            return (T)Rpcs.Find(x => x is T);
        }
    }
}
