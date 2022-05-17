using System;

using UnityEngine;

namespace SimpleVoxelTanks.CommonComponents
{
    public struct Vector3UInt : IEquatable<Vector3UInt>
    {
        public uint X { get; set; }

        public uint Y { get; set; }

        public uint Z { get; set; }

        public Vector3UInt (uint x, uint y, uint z) => (X, Y, Z) = (x, y, z);

        public static uint DiscretDistance (Vector3UInt leftOp, Vector3UInt rightOp) =>
           ((leftOp.X > rightOp.X ? leftOp.X - rightOp.X : rightOp.X - leftOp.X)
            + (leftOp.Y > rightOp.Y ? leftOp.Y - rightOp.Y : rightOp.Y - leftOp.Y)
            + (leftOp.Z > rightOp.Z ? leftOp.Z - rightOp.Z : rightOp.Z - leftOp.Z));

        public static double Distance (Vector3UInt leftOp, Vector3UInt rightOp) => new Vector3UInt(
            leftOp.X > rightOp.X ? leftOp.X - rightOp.X : rightOp.X - leftOp.X,
            leftOp.Y > rightOp.Y ? leftOp.Y - rightOp.Y : rightOp.Y - leftOp.Y,
            leftOp.Z > rightOp.Z ? leftOp.Z - rightOp.Z : rightOp.Z - leftOp.Z).Magnitude();

        public static explicit operator Vector3UInt (Vector3Int vector) => new((uint) vector.x, (uint) vector.y, (uint) vector.z);

        public static implicit operator Vector3 (Vector3UInt vector) => new(vector.X, vector.Y, vector.Z);

        public static implicit operator Vector3Int (Vector3UInt vector) => new((int) vector.X, (int) vector.Y, (int) vector.Z);

        public static Vector3UInt operator - (Vector3UInt leftOp, Vector3UInt rightOp) => new(leftOp.X - rightOp.X,
                                                                                              leftOp.Y - rightOp.Y,
                                                                                              leftOp.Z - rightOp.Z);

        public static bool operator != (Vector3UInt left, Vector3UInt right) => !(left == right);

        public static Vector3UInt operator * (Vector3UInt vector, uint scalar) => new(vector.X * scalar,
                                                                                              vector.Y * scalar,
                                                                                      vector.Z * scalar);

        public static Vector3UInt operator * (uint scalar, Vector3UInt vector) => new(vector.X * scalar,
                                                                                      vector.Y * scalar,
                                                                                      vector.Z * scalar);

        public static Vector3UInt operator + (Vector3UInt vector) => vector;

        public static Vector3UInt operator + (Vector3UInt leftOp, Vector3UInt rightOp) => new(leftOp.X + rightOp.X,
                                                                                              leftOp.Y + rightOp.Y,
                                                                                              leftOp.Z + rightOp.Z);

        public static bool operator == (Vector3UInt left, Vector3UInt right) => left.Equals(right);

        public override bool Equals (object obj) => obj is Vector3UInt @int && Equals(@int);

        public bool Equals (Vector3UInt other) => X == other.X && Y == other.Y && Z == other.Z;

        public override int GetHashCode () => HashCode.Combine(X, Y, Z);

        public double Magnitude () => Math.Sqrt(X * X + Y * Y + Z * Z);

        public override string ToString () => $"(X:{X};Y:{Y};Z:{Z})";
    }
}