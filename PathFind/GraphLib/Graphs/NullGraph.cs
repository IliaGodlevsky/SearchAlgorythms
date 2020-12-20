﻿using GraphLib.Coordinates.Abstractions;
using GraphLib.Graphs.Abstractions;
using GraphLib.Graphs.Serialization.Infrastructure.Info.Collections;
using GraphLib.Vertex;
using GraphLib.Vertex.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GraphLib.Graphs
{
    public sealed class NullGraph : IGraph
    {
        public NullGraph()
        {
            defaultVertex = new DefaultVertex();
            array = new DefaultVertex[] { };
        }

        public IVertex this[ICoordinate position]
        {
            get => defaultVertex;
            set => _ = value;
        }

        public IVertex End { get => defaultVertex; set => _ = value; }

        public int NumberOfVisitedVertices => 0;

        public int ObstacleNumber => 0;

        public int ObstaclePercent => 0;

        public int Size => 0;

        public IVertex Start { get => defaultVertex; set => _ = value; }

        public VertexInfoCollection VertexInfoCollection
            => new VertexInfoCollection(array, DimensionsSizes.ToArray());

        public IEnumerable<int> DimensionsSizes => new int[] { };

        public bool IsDefault => true;

        public IVertex this[int index]
        {
            get => defaultVertex;
            set => _ = value;
        }

        public IEnumerator<IVertex> GetEnumerator()
        {
            return array.Cast<IVertex>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return array.GetEnumerator();
        }

        public string GetFormattedData(string format)
        {
            return string.Empty;
        }

        private readonly IEnumerable<IVertex> array;
        private readonly DefaultVertex defaultVertex;
    }
}