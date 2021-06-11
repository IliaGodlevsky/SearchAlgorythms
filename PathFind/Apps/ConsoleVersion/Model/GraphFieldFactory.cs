﻿using ConsoleVersion.View;
using GraphLib.Base;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using GraphLib.Realizations.Graphs;
using System;

namespace ConsoleVersion.Model
{
    internal sealed class GraphFieldFactory : BaseGraphFieldFactory
    {
        protected override IGraphField GetField()
        {
            return new GraphField();
        }

        /// <summary>
        /// Creates graph field from <paramref name="graph"/>
        /// </summary>
        /// <param name="graph"></param>
        /// <returns>Graph field</returns>
        public override IGraphField CreateGraphField(IGraph graph)
        {
            if (!(graph is Graph2D graph2D))
            {
                string message = $"{nameof(graph)} is not of type {nameof(Graph2D)}";
                throw new ArgumentException(nameof(graph));
            }

            var graphField = new GraphField(graph2D.Width, graph2D.Length);
            graph.ForEach(graphField.Add);

            return graphField;
        }
    }
}
