using System;
using System.Collections.Generic;

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;

namespace SimpleVoxelTanks.DiscretePhysicalSystem
{
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public readonly struct DiscreteTransform : IEquatable<DiscreteTransform>
    {
        public Direction Direction { get; }

        public Vector3UInt Position { get; }

        public DiscreteTransform (Vector3UInt position, Direction direction) => (Position, Direction) = (position, direction);

        public static bool operator != (DiscreteTransform left, DiscreteTransform right) => !(left == right);

        public static bool operator == (DiscreteTransform left, DiscreteTransform right) => left.Equals(right);

        public DiscreteTransform AddDirection (Direction direction) =>
                    new(Direction == direction
                        ? (Vector3UInt) (Position + Direction.DirectionToDeltaPosition())
                        : Position,
                        direction);

        public override bool Equals (object obj) => obj is DiscreteTransform transform && Equals(transform);

        public bool Equals (DiscreteTransform other) => Direction == other.Direction && EqualityComparer<Vector3UInt>.Default.Equals(Position, other.Position);

        public override int GetHashCode () => HashCode.Combine(Direction, Position);
    }

    public static class MoovementExtensions
    {
        public static Vector3Int DirectionToDeltaPosition (this Direction direction) => direction switch
        {
            Direction.Up => new(0, 0, 1),
            Direction.Down => new(0, 0, -1),
            Direction.Left => new(-1, 0, 0),
            Direction.Right => new(1, 0, 0),
            _ => throw new System.NotImplementedException(),
        };

        public static Vector3 DirectionToEulerAngles (this Direction direction) => direction switch
        {
            Direction.Up => Vector3.zero,
            Direction.Down => new(0, 180, 0),
            Direction.Left => new(0, -90, 0),
            Direction.Right => new(0, 90, 0),
            _ => throw new System.NotImplementedException(),
        };

        public static Direction? Vector2IntToDirection (this Vector2Int vector) => vector switch
        {
            { y: > 0 } => Direction.Up,
            { y: < 0 } => Direction.Down,
            { x: > 0 } => Direction.Right,
            { x: < 0 } => Direction.Left,
            _ => null,
        };

        public static Direction? Vector2ToDirection (this Vector2 vector) => vector switch
        {
            { y: > 0 } => Direction.Up,
            { y: < 0 } => Direction.Down,
            { x: > 0 } => Direction.Right,
            { x: < 0 } => Direction.Left,
            _ => null,
        };
    }
}