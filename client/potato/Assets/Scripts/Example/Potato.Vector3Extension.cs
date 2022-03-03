namespace Potato
{
    public static class Vector3Extension
    {
        public static UnityEngine.Vector3 ToVector3(this Torikime.Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }
    }
}

namespace UnityEngine
{
    public static class Vector3Extension
    {
        public static Torikime.Vector3 ToVector3(this UnityEngine.Vector3 v)
        {
            return new Torikime.Vector3()
            {
                X = v.x,
                Y = v.y,
                Z = v.z
            };
        }
    }
}
