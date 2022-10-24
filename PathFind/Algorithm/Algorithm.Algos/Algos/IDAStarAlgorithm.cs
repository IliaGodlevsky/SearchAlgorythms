﻿using Algorithm.Extensions;
using Algorithm.Interfaces;
using Algorithm.Realizations.Heuristic.Distances;
using Algorithm.Realizations.StepRules;
using Common.Extensions.EnumerableExtensions;
using GraphLib.Interfaces;
using GraphLib.Utility;
using NullObject.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Algorithm.Algos.Algos
{
    public class IDAStarAlgorithm : AStarAlgorithm
    {
        private const int PercentToDelete = 4;

        private readonly Dictionary<IVertex, double> deletedVertices;

        private int ToDeleteCount => queue.Count * PercentToDelete / 100;

        public IDAStarAlgorithm(IPathfindingRange endPoints)
            : this(endPoints, new DefaultStepRule(), new ChebyshevDistance())
        {

        }

        public IDAStarAlgorithm(IPathfindingRange endPoints, IStepRule stepRule, IHeuristic function)
            : base(endPoints, stepRule, function)
        {
            deletedVertices = new Dictionary<IVertex, double>(new VertexEqualityComparer());
        }

        protected override IVertex GetNextVertex()
        {
            queue
                .OrderByDescending(heuristics.GetCost)
                .Take(ToDeleteCount)
                .Select(vertex => (Vertex: vertex, Priority: queue.GetPriorityOrInfinity(vertex)))
                .ForEach(item =>
                {
                    queue.TryRemove(item.Vertex);
                    deletedVertices[item.Vertex] = item.Priority;
                });
            var next = base.GetNextVertex();
            if (next.IsNull())
            {
                deletedVertices.ForEach(node => queue.EnqueueOrUpdatePriority(node.Key, node.Value));
                deletedVertices.Clear();
                next = base.GetNextVertex();
            }
            return next;
        }

        protected override void Reset()
        {
            base.Reset();
            deletedVertices.Clear();
        }

        public override string ToString()
        {
            return "IDA* algorithm";
        }
    }
}