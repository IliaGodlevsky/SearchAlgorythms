﻿using GraphLib.Realizations.Graphs;

namespace WPFVersion3D.Model.Axes
{
    internal sealed class Applicate : Axis
    {
        protected override int Order => 0;

        public Applicate(Graph3D<Vertex3D> graph) : base(graph)
        {
        }

        protected override void Offset(Vertex3D vertex, double offset)
        {
            vertex.FieldPosition.OffsetZ = offset;
        }
    }
}