namespace UnityEngine
{
    public static class Vector3Extension
    {
        public static Potato.Vector3 ToVector3(this UnityEngine.Vector3 v)
        {
            return new Potato.Vector3()
            {
                X = v.x,
                Y = v.y,
                Z = v.z
            };
        }
    }
}