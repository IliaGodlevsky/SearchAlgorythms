﻿using Pathfinding.App.Console.DataAccess.Models;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Interface;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using System.Collections.Generic;

namespace Pathfinding.App.Console.Serialization
{
    internal sealed class SerializationInfo
    {
        public Graph2D<Vertex> Graph { get; set; }

        public IReadOnlyList<ICoordinate> Range { get; set; }

        public IList<string> Algorithms { get; set; } = new List<string>();

        public IList<StatisticsModel> Statistics { get; set; } = new List<StatisticsModel>();

        public IList<IReadOnlyList<int>> Costs { get; set; } = new List<IReadOnlyList<int>>();

        public IList<IReadOnlyList<ICoordinate>> Visited { get; set; } = new List<IReadOnlyList<ICoordinate>>();

        public IList<IReadOnlyList<ICoordinate>> Obstacles { get; set; } = new List<IReadOnlyList<ICoordinate>>();

        public IList<IReadOnlyList<ICoordinate>> Ranges { get; set; } = new List<IReadOnlyList<ICoordinate>>();

        public IList<IReadOnlyList<ICoordinate>> Paths { get; set; } = new List<IReadOnlyList<ICoordinate>>();

        public string GraphInformation { get; set; } = string.Empty;
    }
}
