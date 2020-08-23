﻿using GraphLibrary.GraphFactory;
using GraphLibrary;
using GraphLibrary.Vertex;
using GraphLibrary.Graph;
using WinFormsVersion.Vertex;
using WinFormsVersion.Graph;

namespace WinFormsVersion.GraphFactory
{
    public class WinFormsGraphInitializer : AbstractGraphInitializer
    {

        public WinFormsGraphInitializer(VertexInfo[,] info, int placeBetweenTops)
            : base(info, placeBetweenTops)
        {
           
        }

        protected override IVertex CreateVertex(VertexInfo info)
        {
            return new WinFormsVertex(info);
        }

        public override AbstractGraph GetGraph()
        {
            return new WinFormsGraph(vertices);
        }

        protected override void SetGraph(int width, int height)
        {
            vertices = new WinFormsVertex[width, height];
        }
    }
}