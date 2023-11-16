﻿using Pathfinding.GraphLib.Core.Interface;
using System;
using System.Collections.Generic;

namespace Pathfinding.App.Console.DataAccess.Entities
{
    internal class VisitedVerticesEntity : IEntity<int>
    {
        public int Id { get; set; }

        public Guid AlgorithmId { get; set; }

        public IList<ICoordinate> Visited { get; set; }
    }
}
