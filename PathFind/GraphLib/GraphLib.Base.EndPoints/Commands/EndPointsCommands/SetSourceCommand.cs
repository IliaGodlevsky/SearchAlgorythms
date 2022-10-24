﻿using Common.Attrbiutes;
using GraphLib.Base.EndPoints.BaseCommands;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using NullObject.Extensions;

namespace GraphLib.Base.EndPoints.Commands.EndPointsCommands
{
    [Order(4)]
    internal sealed class SetSourceCommand : BasePathfindingRangeCommand
    {
        public SetSourceCommand(BasePathfindingRange endPoints)
            : base(endPoints)
        {
        }

        public override void Execute(IVertex vertex)
        {
            range.Source = vertex;
            Source.VisualizeAsSource();
        }

        public override bool CanExecute(IVertex vertex)
        {
            return range.Source.IsNull()
                && range.CanBeInPathfindingRange(vertex);
        }
    }
}