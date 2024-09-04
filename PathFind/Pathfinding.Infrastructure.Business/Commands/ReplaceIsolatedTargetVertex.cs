﻿using Pathfinding.Domain.Interface;
using Pathfinding.Infrastructure.Data.Extensions;
using Pathfinding.Service.Interface.Commands;
using Pathfinding.Shared;
using System.Linq;

namespace Pathfinding.Infrastructure.Business.Commands
{
    [Order(5), Group(Constants.IncludeCommands)]
    public sealed class ReplaceIsolatedTargetVertex<TVertex> : IPathfindingRangeCommand<TVertex>
        where TVertex : IVertex
    {
        public void Execute(IPathfindingRange<TVertex> range, TVertex vertex)
        {
            range.Target = default;
            range.Target = vertex;
        }

        public bool CanExecute(IPathfindingRange<TVertex> range, TVertex vertex)
        {
            return range.Target != null
                && range.Target.IsIsolated()
                && range.CanBeInRange(vertex);
        }
    }
}