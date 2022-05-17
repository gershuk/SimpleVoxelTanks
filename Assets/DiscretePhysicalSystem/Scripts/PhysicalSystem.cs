using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.DiscretePhysicalSystem
{
    public static class PhysicalSystem
    {
        private static ColliderGrid _colliderGrid;

        public static ColliderGrid ColliderGrid => _colliderGrid;

        //fucking unity https://answers.unity.com/questions/1151780/is-there-an-equivalent-of-timeframecount-for-physi.html
        public static uint FixedUpdateFrameNumber => (uint) Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);

        public static void Init (Vector3UInt size) => _colliderGrid = new(size);

        public static void Init (uint x, uint y, uint z) => Init(new(x, y, z));
    }
}