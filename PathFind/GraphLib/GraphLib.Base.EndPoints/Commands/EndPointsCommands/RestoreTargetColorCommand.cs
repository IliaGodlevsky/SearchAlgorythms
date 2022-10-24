﻿using Common.Attrbiutes;
using GraphLib.Base.EndPoints.BaseCommands;
using GraphLib.Extensions;
using GraphLib.Interfaces;

namespace GraphLib.Base.EndPoints.Commands.EndPointsCommands
{
    [Order(1)]
    internal sealed class RestoreTargetColorCommand : BasePathfindingRangeCommand
    {
        public RestoreTargetColorCommand(BasePathfindingRange endPoints)
            : base(endPoints)
        {
        }

        public override void Execute(IVertex vertex)
        {
            vertex.AsVisualizable().VisualizeAsTarget();
        }

        public override bool CanExecute(IVertex vertex)
        {
            return vertex.Equals(range.Target);
        }
    }
}