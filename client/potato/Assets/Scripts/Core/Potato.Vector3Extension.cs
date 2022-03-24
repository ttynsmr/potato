namespace Potato
{
    public static class Vector3Extension
    {
        public static UnityEngine.Vector3 ToVector3(this Potato.Vector3 v)
        {
            return new UnityEngine.Vector3(v.X, v.Y, v.Z);
        }
    }
}
