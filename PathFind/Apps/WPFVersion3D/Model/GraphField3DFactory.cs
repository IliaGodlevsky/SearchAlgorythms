﻿using GraphLib.Interfaces;
using GraphLib.Realizations.Graphs;

namespace WPFVersion3D.Model
{
    internal sealed class GraphField3DFactory : IGraphFieldFactory<Graph3D<Vertex3D>, Vertex3D, GraphField3D>
    {
        public GraphField3D CreateGraphField(Graph3D<Vertex3D> graph)
        {
            return new GraphField3D(graph);
        }
    }
}
