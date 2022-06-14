﻿using Common.Attrbiutes;
using GraphLib.Base.EndPoints.BaseCommands;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using NullObject.Extensions;

namespace GraphLib.Base.EndPoints.Commands.EndPointsCommands
{
    [Order(6)]
    internal sealed class SetTargetCommand : BaseEndPointsCommand
    {
        public SetTargetCommand(BaseEndPoints endPoints)
            : base(endPoints)
        {
        }

        public override void Execute(IVertex vertex)
        {
            endPoints.Target = vertex;
            Target.VisualizeAsTarget();
        }

        public override bool CanExecute(IVertex vertex)
        {
            return !endPoints.Source.IsNull()
                && endPoints.Target.IsNull()
                && endPoints.CanBeEndPoint(vertex);
        }
    }
}
