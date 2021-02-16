﻿using Algorithm.Extensions;
using GraphLib.Common.NullObjects;
using GraphLib.Interface;
using Plugins.DijkstraALgorithm;
using System.ComponentModel;

namespace Plugins.AStarAlgorithm
{
    [Description("A* algorithm")]
    public class AStarAlgorithm : DijkstraAlgorithm
    {
        public AStarAlgorithm() : this(new NullGraph())
        {

        }

        public AStarAlgorithm(IGraph graph) : base(graph)
        {

        }

        protected virtual double CalculateHeuristic(IVertex vertex)
        {
            return vertex.CalculateChebyshevDistanceTo(endPoints.End);
        }

        protected override double GetVertexRelaxedCost(IVertex neighbour)
        {
            return base.GetVertexRelaxedCost(neighbour) + CalculateHeuristic(CurrentVertex);
        }
    }
}