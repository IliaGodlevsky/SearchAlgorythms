﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.GraphLib.Core.Interface.Extensions
{
    public static class IGraphExtensions
    {
        public static int GetObstaclePercent<TVertex>(this IGraph<TVertex> self)
            where TVertex : IVertex
        {
            return (int)Math.Round(self.Count == 0 ? 0 : self.GetObstaclesCount() * 100.0 / self.Count);
        }

        public static void ApplyCosts<T>(this IEnumerable<T> graph, IEnumerable<int> costs)
            where T : IVertex
        {
            foreach (var item in graph.Zip(costs, (v, p) => (Vertex: v, Price: p)))
            {
                item.Vertex.Cost = item.Vertex.Cost.SetCost(item.Price);
            }
        }

        public static IEnumerable<TVertex> GetObstacles<TVertex>(this IGraph<TVertex> graph)
            where TVertex : IVertex
        {
            return graph.Where(vertex => vertex.IsObstacle);
        }

        public static IEnumerable<ICoordinate> GetCoordinates<TVertex>(this IEnumerable<TVertex> graph)
            where TVertex : IVertex
        {
            return graph.Select(vertex => vertex.Position);
        }

        public static IEnumerable<ICoordinate> GetObstaclesCoordinates<TVertex>(this IGraph<TVertex> graph)
            where TVertex : IVertex
        {
            return graph.GetObstacles().Select(vertex => vertex.Position);
        }

        public static IEnumerable<ICoordinate> GetNotObstaclesCoordinates<TVertex>(this IGraph<TVertex> graph)
            where TVertex : IVertex
        {
            return graph.Where(vertex => !vertex.IsObstacle).Select(vertex => vertex.Position);
        }

        public static int GetObstaclesCount<TVertex>(this IGraph<TVertex> self)
            where TVertex : IVertex
        {
            return self.GetObstacles().Count();
        }

        public static int GetNumberOfNotIsolatedVertices<TVertex>(this IGraph<TVertex> graph)
            where TVertex : IVertex
        {
            return graph.Where(vertex => !vertex.IsIsolated()).Count();
        }

        public static string GetStringRepresentation<TVertex>(this IGraph<TVertex> graph,
            string format = "Obstacle percent: {0} ({1}/{2})")
            where TVertex : IVertex
        {
            const string LargeSpace = "   ";
            var dimensionNames = new[] { "Width", "Length", "Height" };
            int obstacles = graph.GetObstaclesCount();
            int obstaclesPercent = graph.GetObstaclePercent();
            string Zip(string name, int size) => $"{name}: {size}";
            var zipped = dimensionNames.Zip(graph.DimensionsSizes, Zip);
            string joined = string.Join(LargeSpace, zipped);
            string graphParams = string.Format(format,
                obstaclesPercent, obstacles, graph.Count);
            return string.Join(LargeSpace, joined, graphParams);
        }
    }
}