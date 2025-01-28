﻿using Pathfinding.Domain.Interface;

namespace Pathfinding.Infrastructure.Business.Layers
{
    public sealed class Layers : List<ILayer>, ILayer
    {
        public Layers(params ILayer[] layers)
            : base(layers)
        {

        }

        public Layers(IEnumerable<ILayer> layers)
            : this(layers.ToArray())
        {

        }

        public void Overlay(IGraph<IVertex> graph)
        {
            ForEach(layer => layer.Overlay(graph));
        }
    }
}
