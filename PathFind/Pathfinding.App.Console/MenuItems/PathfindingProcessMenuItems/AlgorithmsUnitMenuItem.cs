﻿using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Units;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.Logging.Interface;

namespace Pathfinding.App.Console.MenuItems.PathfindingProcessMenuItems
{
    [HighestPriority]
    internal sealed class AlgorithmsUnitMenuItem
        : UnitDisplayMenuItem<AlgorithmChooseUnit>, IConditionedMenuItem
    {
        private readonly IPathfindingRangeBuilder<Vertex> builder;

        public AlgorithmsUnitMenuItem(IInput<int> intInput,
            AlgorithmChooseUnit unit,
            IPathfindingRangeBuilder<Vertex> builder,
            ILog log)
            : base(intInput, unit, log)
        {
            this.builder = builder;
        }

        public bool CanBeExecuted()
        {
            return builder.Range.HasSourceAndTargetSet()
                && !builder.Range.HasIsolators();
        }

        public override string ToString()
        {
            return Languages.FindPath;
        }
    }
}
