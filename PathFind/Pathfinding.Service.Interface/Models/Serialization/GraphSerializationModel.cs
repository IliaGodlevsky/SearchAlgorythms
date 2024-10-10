﻿using System.Collections.Generic;

namespace Pathfinding.Service.Interface.Models.Serialization
{
    public record GraphSerializationModel
    {
        public string Name { get; set; }

        public string SmoothLevel { get; set; }

        public string Neighborhood { get; set; }

        public IReadOnlyList<int> DimensionSizes { get; set; }

        public IReadOnlyCollection<VertexSerializationModel> Vertices { get; set; }
    }
}
