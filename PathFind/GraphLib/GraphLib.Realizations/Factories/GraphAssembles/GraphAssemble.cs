﻿using Common.Extensions.EnumerableExtensions;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using GraphLib.Interfaces.Factories;
using Random.Extensions;
using Random.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using ValueRange;
using ValueRange.Extensions;

namespace GraphLib.Realizations.Factories.GraphAssembles
{
    public class GraphAssemble : IGraphAssemble
    {
        protected readonly IVertexCostFactory costFactory;
        protected readonly ICoordinateFactory coordinateFactory;
        protected readonly IVertexFactory vertexFactory;
        protected readonly IGraphFactory graphFactory;
        protected readonly INeighborhoodFactory neighbourhoodFactory;
        protected readonly IRandom random;
        protected readonly InclusiveValueRange<int> percentRange;

        public GraphAssemble(
            IVertexFactory vertexFactory,
            ICoordinateFactory coordinateFactory,
            IGraphFactory graphFactory,
            IVertexCostFactory costFactory,
            INeighborhoodFactory neighbourhoodFactory,
            IRandom random)
        {
            this.vertexFactory = vertexFactory;
            this.coordinateFactory = coordinateFactory;
            this.graphFactory = graphFactory;
            this.costFactory = costFactory;
            this.neighbourhoodFactory = neighbourhoodFactory;
            this.random = random;
            percentRange = new InclusiveValueRange<int>(99, 0);
        }

        public virtual IGraph AssembleGraph(int obstaclePercent, IReadOnlyList<int> graphDimensionsSizes)
        {
            int graphSize = graphDimensionsSizes.AggregateOrDefault((x, y) => x * y);
            int percentOfObstacles = percentRange.ReturnInRange(obstaclePercent);
            int numberOfObstacles = (int)Math.Round(graphSize * percentOfObstacles / 100.0, 0);
            var regulars = Enumerable.Repeat(false, graphSize - numberOfObstacles);
            var obstacles = Enumerable.Repeat(true, numberOfObstacles);
            var obstaclesMatrix = regulars.Concat(obstacles).Shuffle(random.NextInt).ToReadOnly();
            var vertices = new IVertex[graphSize];
            for (int i = 0; i < graphSize; i++)
            {
                var coordinateValues = graphDimensionsSizes.ToCoordinates(i);
                var coordinate = coordinateFactory.CreateCoordinate(coordinateValues);
                var neighbourhood = neighbourhoodFactory.CreateNeighborhood(coordinate);
                var vertex = vertexFactory.CreateVertex(neighbourhood, coordinate);
                vertex.Cost = costFactory.CreateCost();
                vertex.IsObstacle = obstaclesMatrix[i];
                vertices[i] = vertex;
            }

            return graphFactory.CreateGraph(vertices, graphDimensionsSizes);
        }

        public override string ToString()
        {
            return "Random cost graph assemble";
        }
    }
}