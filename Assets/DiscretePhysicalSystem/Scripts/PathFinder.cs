#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;

using SimpleVoxelTanks.CommonComponents;

using UnityEngine;
using UnityEngine.Profiling;

namespace SimpleVoxelTanks.DiscretePhysicalSystem
{
    public readonly struct WayPoint
    {
        public uint FixedFrameNumber { get; }

        public Vector3UInt Position { get; }

        public WayPoint (Vector3UInt position, uint fixedFrameNumber) =>
            (Position, FixedFrameNumber) = (position, fixedFrameNumber);
    }

    public static partial class PathFinder
    {
        private enum State
        {
            Ignore = 0,
            Move = 1,
            MoveAndDestory = 2,
        }

        internal static (Vector3UInt size, WayPoint?[,,] vistedFrom, SortedSet<WayPoint> wayPointsQueue) InitAStarData (DiscretPhysicalBody body,
                                                                                                                        HashSet<Vector3UInt> to)
        {
            if (PhysicalSystem.ColliderGrid == null)
                throw new NullReferenceException($"PhysicalSystem.ColliderGrid is null");

            var size = PhysicalSystem.ColliderGrid.Size;
            var vistedFrom = new WayPoint?[size.X, size.Y, size.Z];

            vistedFrom[body.DiscreteTransform.Position.X, body.DiscreteTransform.Position.Y, body.DiscreteTransform.Position.Z] =
                new(body.DiscreteTransform.Position, 0);

            WayPointComparer comparer = new(to, body.MovementTicks + body.RotationTicks);
            SortedSet<WayPoint> set = new(comparer);
            set.Add(new(body.DiscreteTransform.Position, 0));

            return (size, vistedFrom, set);
        }

        public static List<Vector3UInt>? AStar (DiscretPhysicalBody body,
                                              HashSet<Vector3UInt> to,
                                              Vector3Int[] moveSet,
                                              bool canDestory = false,
                                              uint tickToDestory = 0,
                                              int hpDmgPerTick = 1)
        {
            Profiler.BeginSample("Init");
            (var size, var vistedFrom, var wayPointsQueue) = InitAStarData(body, to);
            Profiler.EndSample();

            if (PhysicalSystem.ColliderGrid == null)
                throw new NullReferenceException($"PhysicalSystem.ColliderGrid is null");

            while (wayPointsQueue.Count > 0)
            {
                Profiler.BeginSample("Get Way Point");
                var wayPoint = wayPointsQueue.First();
                wayPointsQueue.Remove(wayPoint);

                if (wayPoint.Position == body.DiscreteTransform.Position && vistedFrom[wayPoint.Position.X,
                                                                                       wayPoint.Position.Y,
                                                                                       wayPoint.Position.Z].HasValue == false)
                {
                    throw new NullReferenceException($"It's not start position, but previous value null.");
                }

                var lastDelta = wayPoint.Position == body.DiscreteTransform.Position
                                ? body.DiscreteTransform.Direction.DirectionToDeltaPosition()
                                : (Vector3Int) wayPoint.Position - vistedFrom[wayPoint.Position.X,
                                                                              wayPoint.Position.Y,
                                                                              wayPoint.Position.Z]!.Value.Position;
                Profiler.EndSample();

                foreach (var move in moveSet)
                {
                    Profiler.BeginSample("Check position");
                    var possiblePosition = wayPoint.Position + move;

                    if ((possiblePosition.x < 0 || possiblePosition.x >= size.X)
                        || (possiblePosition.y < 0 || possiblePosition.y >= size.Y)
                        || (possiblePosition.z < 0 || possiblePosition.z >= size.Z))
                    {
                        continue;
                    }

                    var newPosition = (Vector3UInt) possiblePosition;
                    var moveTicks = wayPoint.FixedFrameNumber + body.MovementTicks;

                    if (lastDelta != move)
                        moveTicks += body.RotationTicks;
                    Profiler.EndSample();

                    var state = State.Ignore;
                    Profiler.BeginSample("Check Destory");
                    var discretPhysicalBody = PhysicalSystem.ColliderGrid[newPosition].DiscretPhysicalBody;
                    var damageableObject = discretPhysicalBody != null
                                           ? discretPhysicalBody.gameObject.GetComponent<DamageableObject>()
                                           : null;
                    var moveAndDestoryTicks = uint.MaxValue;
                    var oldValue = vistedFrom[newPosition.X, newPosition.Y, newPosition.Z];
                    if (canDestory && damageableObject != null)
                    {
                        moveAndDestoryTicks = moveTicks + tickToDestory * (uint) ((damageableObject.HealthPoints - 1) / hpDmgPerTick + 1);
                        if (vistedFrom[newPosition.X, newPosition.Y, newPosition.Z] == null ||
                            vistedFrom[newPosition.X, newPosition.Y, newPosition.Z]!.Value.FixedFrameNumber > moveAndDestoryTicks)
                        {
                            vistedFrom[newPosition.X, newPosition.Y, newPosition.Z] = new(wayPoint.Position, moveAndDestoryTicks);
                            state = State.MoveAndDestory;
                        }
                    }
                    Profiler.EndSample();

                    Profiler.BeginSample("Check Move");
                    if ((vistedFrom[newPosition.X, newPosition.Y, newPosition.Z] == null
                         || vistedFrom[newPosition.X, newPosition.Y, newPosition.Z]!.Value.FixedFrameNumber > moveTicks)
                         && PhysicalSystem.ColliderGrid.CanSetCell(body, newPosition, moveTicks))
                    {
                        vistedFrom[newPosition.X, newPosition.Y, newPosition.Z] = new(wayPoint.Position, moveTicks);
                        state = State.Move;
                    }
                    Profiler.EndSample();

                    if (state is State.Move or State.MoveAndDestory)
                    {
                        Profiler.BeginSample("Update Set");
                        if (oldValue != null)
                            wayPointsQueue.Remove(new WayPoint(newPosition, oldValue.Value.FixedFrameNumber));
                        wayPointsQueue.Add(new(newPosition, state switch
                        {
                            State.Move => moveTicks,
                            State.MoveAndDestory => moveAndDestoryTicks,
                            _ or State.Ignore => throw new System.NotImplementedException(),
                        }));
                        Profiler.EndSample();

                        Profiler.BeginSample("Calc way");
                        if (to.Contains(newPosition))
                        {
                            var currentPosition = newPosition;
                            List<Vector3UInt> path = new() { currentPosition };
                            while (currentPosition != body.DiscreteTransform.Position)
                            {
                                var from = vistedFrom[currentPosition.X, currentPosition.Y, currentPosition.Z];
                                if (from.HasValue == false)
                                    throw new NullReferenceException($"{nameof(from)} is null");
                                currentPosition = from.Value.Position;
                                path.Add(currentPosition);
                            }

                            return path;
                        }
                        Profiler.EndSample();
                    }
                }
            }
            return null;
        }
    }

    public class WayPointComparer : IComparer<WayPoint>
    {
        private readonly HashSet<Vector3UInt> _targetPoints;
        private readonly uint _tickPerUnit;

        public WayPointComparer (HashSet<Vector3UInt> targetPoints, uint tickPerUnit) =>
            (_targetPoints, _tickPerUnit) = (targetPoints, tickPerUnit);

        public int Compare (WayPoint x, WayPoint y)
        {
            var comp = 0;
            foreach (var targetPoint in _targetPoints)
                comp = F(x, targetPoint).CompareTo(F(y, targetPoint));

            if (comp == 0 && x.Position != y.Position)
                comp = -1;

            return comp;
        }

        public uint F (WayPoint wayPoint, Vector3UInt target) =>
            wayPoint.FixedFrameNumber + Vector3UInt.DiscretDistance(wayPoint.Position, target) * 3 * _tickPerUnit;
    }
}