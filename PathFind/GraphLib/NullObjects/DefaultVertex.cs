﻿using GraphLib.Interface;
using GraphLib.VertexCost;
using System.Collections.Generic;

namespace GraphLib.NullObjects
{
    public sealed class DefaultVertex : IVertex
    {
        public DefaultVertex()
        {

        }

        public bool IsEnd { get => true; set => _ = value; }

        public bool IsObstacle { get => true; set => _ = value; }

        public bool IsStart { get => true; set => _ = value; }

        public bool IsVisited { get => true; set => _ = value; }

        public IVertexCost Cost { get => new Cost(default); set => _ = value; }

        public IList<IVertex> Neighbours { get => new List<IVertex>(); set => _ = value; }

        public IVertex ParentVertex { get => new DefaultVertex(); set => _ = value; }

        public double AccumulatedCost { get => double.PositiveInfinity; set => _ = value; }

        public ICoordinate Position { get => new DefaultCoordinate(); set => _ = value; }

        public bool IsDefault => true;

        public void MarkAsEnd()
        {

        }

        public void MarkAsObstacle()
        {

        }

        public void MarkAsPath()
        {

        }

        public void MarkAsSimpleVertex()
        {

        }

        public void MarkAsStart()
        {

        }

        public void MarkAsVisited()
        {

        }

        public void MarkAsEnqueued()
        {

        }

        public void MakeUnweighted()
        {

        }

        public void MakeWeighted()
        {

        }
    }
}
