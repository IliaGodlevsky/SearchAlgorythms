﻿using Pathfinding.App.Console.DataAccess.Entities;

namespace Pathfinding.App.Console.DataAccess
{
    internal sealed class GraphRangeInMemoryStorage : InMemoryStorage<GraphRangeEntity, int>
    {
        private static int id = 0;

        protected override int NextId => id++;
    }
}
