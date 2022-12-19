﻿using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Shared.Primitives.Attributes;

namespace Pathfinding.App.Console.MenuItems.PathfindingRangeMenuItems
{
    [Order(2)]
    internal sealed class ClearPathfindingRangeMenuItem : IMenuItem
    {
        private readonly IPathfindingRangeBuilder<Vertex> rangeBuilder;

        public ClearPathfindingRangeMenuItem(IPathfindingRangeBuilder<Vertex> rangeBuilder)
        {
            this.rangeBuilder = rangeBuilder;
        }

        public bool CanBeExecuted() => true;

        public void Execute()
        {
            rangeBuilder.Undo();
        }

        public override string ToString()
        {
            return Languages.ClearPathfindingRange;
        }
    }
}
