using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torikime
{

    public interface IRpc { }

    public static class RpcBuilder
    {
        public static List<IRpc> Build()
        {
            rpcs.Add(new Auth.Login.Rpc());
            rpcs.Add(new Channel.Create.Rpc());
            rpcs.Add(new Channel.Search.Rpc());
            rpcs.Add(new Chat.SendMessage.Rpc());
            rpcs.Add(new Chat.SendStamp.Rpc());
            return rpcs;
        }

        static List<IRpc> rpcs = new List<IRpc>();
    }
}