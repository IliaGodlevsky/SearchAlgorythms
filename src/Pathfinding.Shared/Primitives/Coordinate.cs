﻿using Pathfinding.Shared.Extensions;
using System.Runtime.CompilerServices;

namespace Pathfinding.Shared.Primitives
{
    public readonly struct Coordinate : IEquatable<Coordinate>
    {
        public static readonly Coordinate Empty = new();

        private readonly string toString;
        private readonly int hashCode;

        public readonly int[] CoordinatesValues { get; }

        public readonly int Count => CoordinatesValues.Length;

        public readonly int this[int index] => CoordinatesValues[index];

        public Coordinate(int numberOfDimensions, IReadOnlyList<int> coordinates)
        {
            CoordinatesValues = coordinates.TakeOrDefault(numberOfDimensions).ToArray();
            toString = $"({string.Join(",", CoordinatesValues)})";
            hashCode = CoordinatesValues.AggregateOrDefault(HashCode.Combine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coordinate(IReadOnlyList<int> coordinates)
            : this(coordinates.Count, coordinates)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coordinate(IEnumerable<int> coordinates)
            : this(coordinates.ToArray())
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Coordinate(params int[] coordinates)
            : this((IReadOnlyList<int>)coordinates)
        {

        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Coordinate other)
        {
            return other.GetHashCode().Equals(GetHashCode());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals(object pos)
        {
            return pos is Coordinate coord && Equals(coord);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly int GetHashCode() => hashCode;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => toString;

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !(left == right);
        }
    }
}