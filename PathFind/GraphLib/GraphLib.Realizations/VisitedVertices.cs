﻿using GraphLib.Interfaces;
using NullObject.Extensions;
using System.Collections.Generic;

namespace GraphLib.Realizations
{
    public sealed class VisitedVertices : IVisitedVertices
    {
        private readonly HashSet<IVertex> visitedVertices;

        public VisitedVertices()
        {
            visitedVertices = new HashSet<IVertex>();
        }

        public void Visit(IVertex vertex)
        {
            if (!vertex.IsNull())
            {
                visitedVertices.Add(vertex);
            }
        }

        public bool IsNotVisited(IVertex vertex)
        {
            return !visitedVertices.Contains(vertex);
        }

        public void Clear()
        {
            visitedVertices.Clear();
        }
    }
}