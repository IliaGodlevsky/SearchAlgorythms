﻿using GraphLibrary.GraphCreating;
using GraphLibrary.Graphs;
using GraphLibrary.Graphs.Interface;
using GraphLibrary.Vertex.Interface;
using GraphLibrary.ViewModel.Interface;
using System;

namespace GraphLibrary.ViewModel
{
    public abstract class GraphCreatingModel : IModel
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int ObstaclePercent { get; set; }

        public GraphCreatingModel(IMainModel model)
        {
            this.model = model;
            graph = NullGraph.Instance;
        }

        public virtual void CreateGraph(Func<IVertex> vertexFactory)
        {
            var factory = new GraphFactory(new GraphParametres(Width, Height, ObstaclePercent));
            graph = factory.CreateGraph(vertexFactory);
            model.SetGraph(graph);
        }

        protected IMainModel model;
        protected IGraph graph;
    }
}
