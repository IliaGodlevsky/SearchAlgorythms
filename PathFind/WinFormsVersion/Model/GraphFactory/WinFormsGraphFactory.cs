﻿using GraphLibrary.GraphFactory;
using GraphLibrary.Vertex;
using GraphLibrary.Graph;
using WinFormsVersion.Vertex;
using WinFormsVersion.Graph;
using GraphLibrary.Common.Constants;

namespace WinFormsVersion.GraphFactory
{
    internal class WinFormsGraphFactory : AbstractGraphFactory
    {
        public WinFormsGraphFactory(int percentOfObstacles,
            int width, int height, int placeBetweenButtons) : base(percentOfObstacles,
                width, height, placeBetweenButtons)
        {
            
        }

        protected override IVertex CreateVertex() => new WinFormsVertex { Cost = VertexValueRange.GetRandomVertexValue() };

        public override AbstractGraph GetGraph() => new WinFormsGraph(vertices);
    }
}
