﻿using GraphLib.Interface;
using GraphLib.Realizations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsVersion.Model;

namespace WindowsFormsVersion.View
{
    internal class WinFormsGraphField : UserControl, IGraphField
    {
        public IEnumerable<IVertex> Vertices => Controls.OfType<IVertex>();

        public WinFormsGraphField()
        {
            distanceBetweenVertices = Constants.DistanceBetweenVertices + Constants.VertexSize;
        }

        public void Add(IVertex vertex)
        {
            if (vertex.Position is Coordinate2D coordinate && vertex is Vertex winFormsVertex)
            {
                var xCoordinate = coordinate.X * distanceBetweenVertices;
                var yCoordinate = coordinate.Y * distanceBetweenVertices;

                winFormsVertex.Location = new Point(xCoordinate, yCoordinate);

                Controls.Add(winFormsVertex);
            }
            else
            {
                string message = $"Must be 2D vertex";
                throw new ArgumentException(message);
            }
        }

        public void Clear()
        {
            Controls.Clear();
        }

        private readonly int distanceBetweenVertices;
    }
}
