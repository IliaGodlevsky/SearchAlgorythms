﻿using Common.Interfaces;
using GraphLib.VertexCost;
using System.Collections.Generic;

namespace GraphLib.Interface
{
    /// <summary>
    /// Represents a vertex of graph
    /// </summary>
    public interface IVertex : IDefault
    {
        bool IsObstacle { get; set; }

        Cost Cost { get; set; }

        IList<IVertex> Neighbours { get; set; }

        ICoordinate Position { get; set; }

        void MarkAsEnd();

        void MarkAsSimpleVertex();

        void MarkAsObstacle();

        void MarkAsPath();

        void MarkAsStart();

        void MarkAsVisited();

        void MarkAsEnqueued();

        void MakeUnweighted();

        void MakeWeighted();
    }
}