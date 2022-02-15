﻿using Common.Attrbiutes;
using GraphLib.Base.EndPoints.Attributes;
using GraphLib.Base.EndPoints.BaseCommands;
using GraphLib.Base.EndPoints.Commands.VerticesCommands;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using NullObject.Extensions;

namespace GraphLib.Base.EndPoints.Commands.EndPointsCommands
{
    [Attachment(typeof(SetEndPointsCommands)), Order(7)]
    internal sealed class ReplaceIsolatedTargetVertexCommand : BaseEndPointsCommand
    {
        public ReplaceIsolatedTargetVertexCommand(BaseEndPoints endPoints)
            : base(endPoints)
        {
        }

        public override void Execute(IVertex vertex)
        {
            (endPoints.Target as IVisualizable)?.VisualizeAsRegular();
            endPoints.Target = vertex;
            (vertex as IVisualizable)?.VisualizeAsTarget();
        }

        public override bool IsTrue(IVertex vertex)
        {
            return endPoints.Target.IsIsolated()
                && !endPoints.Target.IsNull()
                && !endPoints.IsEndPoint(vertex);
        }
    }
}
