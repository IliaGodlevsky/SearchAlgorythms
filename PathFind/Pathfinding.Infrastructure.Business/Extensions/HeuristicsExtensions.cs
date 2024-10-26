﻿using Pathfinding.Infrastructure.Business.Algorithms.Heuristics;
using Pathfinding.Service.Interface;

namespace Pathfinding.Infrastructure.Business.Extensions
{
    public static class HeuristicsExtensions
    {
        public static WeightedHeuristic ToWeighted(this IHeuristic heuristic, double? weight)
        {
            if (weight == null)
            {
                new WeightedHeuristic(heuristic, 0);
            }
            return new WeightedHeuristic(heuristic, weight.Value);
        }
    }
}
