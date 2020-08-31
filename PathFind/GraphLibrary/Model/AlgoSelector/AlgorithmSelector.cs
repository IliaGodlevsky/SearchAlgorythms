﻿using GraphLibrary.Algorithm;
using GraphLibrary.DistanceCalculator;
using GraphLibrary.AlgorithmEnum;
using GraphLibrary.Graph;
using GraphLibrary.PathFindAlgorithm;
using GraphLibrary.Vertex;

namespace GraphLibrary.AlgoSelector
{
    
    public static class AlgorithmSelector
    {
        private static double AStartRelaxFunction(IVertex vertex, 
            IVertex neighbour, IVertex destination)
        {
            return neighbour.Cost + vertex.AccumulatedCost 
                + Distance.GetChebyshevDistance(neighbour, destination);
        }

        public static IPathFindAlgorithm GetPathFindAlgorithm(Algorithms algorithms, AbstractGraph graph)
        {
            switch (algorithms)
            {
                case Algorithms.WidePathFind: return new WidePathFindAlgorithm(graph);
                case Algorithms.DeepPathFind: return new DeepPathFindAlgorithm(graph);
                case Algorithms.DijkstraAlgorithm: return new DijkstraAlgorithm(graph);
                case Algorithms.AStarAlgorithm: return new DijkstraAlgorithm(graph)
                {
                    RelaxFunction = (neighbour, vertex) => AStartRelaxFunction(vertex, neighbour, graph.End)
                };
                case Algorithms.DistanceGreedyAlgorithm: return new GreedyAlgorithm(graph)
                {
                    GreedyFunction = vertex => Distance.GetEuclideanDistance(vertex, graph.End)
                };
                case Algorithms.ValueGreedyAlgorithm: return new GreedyAlgorithm(graph)
                {
                    GreedyFunction = vertex => vertex.Cost
                };
                case Algorithms.ValueDistanceGreedyAlgorithm: return new GreedyAlgorithm(graph)
                {
                    GreedyFunction = vertex => vertex.Cost + Distance.GetEuclideanDistance(vertex, graph.End)
                };
                default: return null;
            }
        }
    }
}
