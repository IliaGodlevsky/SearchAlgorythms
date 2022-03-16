﻿using Commands.Interfaces;
using Common.Attrbiutes;
using GraphLib.Base.EndPoints.BaseCommands;
using GraphLib.Base.EndPoints.Commands.EndPointsCommands;
using GraphLib.Base.EndPoints.Commands.VerticesCommands;
using GraphLib.Interfaces;

namespace GraphLib.Base.EndPoints.Commands.UndoCommands
{
    [AttachedTo(typeof(SetEndPointsCommands))]
    internal sealed class UndoSetTargetCommand : BaseEndPointsUndoCommand
    {
        public UndoSetTargetCommand(BaseEndPoints endPoints) : base(endPoints)
        {
            unsetTargetCommand = new UnsetTargetCommand(endPoints);
        }

        public override void Undo()
        {
            unsetTargetCommand.Execute(Target);
        }

        private readonly IExecutable<IVertex> unsetTargetCommand;
    }
}