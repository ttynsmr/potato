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
            return new List<IRpc>();
        }
    }
}