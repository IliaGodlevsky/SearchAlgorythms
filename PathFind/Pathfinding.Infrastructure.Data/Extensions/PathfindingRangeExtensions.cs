﻿using Pathfinding.Domain.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.Infrastructure.Data.Extensions
{
    public static class PathfindingRangeExtensions
    {
        public static bool CanBeInRange<TVertex>(this ICollection<TVertex> range, TVertex vertex)
            where TVertex : IVertex
        {
            return !vertex.IsIsolated() && !range.Contains(vertex);
        }

        public static bool HasSourceAndTargetSet<TVertex>(this IPathfindingRange<TVertex> range)
            where TVertex : IVertex
        {
            return range.Source?.IsIsolated() == false && range.Target?.IsIsolated() == false;
        }

        public static bool HasIsolators<TVertex>(this IPathfindingRange<TVertex> range)
            where TVertex : IVertex
        {
            return range.Any(vertex => vertex.IsIsolated());
        }

        public static IEnumerable<IVertex> AsEnumerable<TVertex>(this IPathfindingRange<TVertex> range)
            where TVertex : IVertex
        {
            foreach (var vertex in range)
            {
                yield return vertex;
            }
        }
    }
}
