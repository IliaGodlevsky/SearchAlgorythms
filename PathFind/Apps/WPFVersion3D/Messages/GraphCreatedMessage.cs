﻿using GraphLib.Interfaces;

namespace WPFVersion3D.Messages
{
    internal sealed class GraphCreatedMessage
    {
        public IGraph Graph { get; }

        public GraphCreatedMessage(IGraph graph)
        {
            Graph = graph;
        }
    }
}