﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Serialization;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Extensions;
using Pathfinding.Logging.Interface;

namespace Pathfinding.App.Console.MenuItems.GraphMenuItems
{
    [LowPriority]
    internal sealed class RecieveGraphMenuItem : ImportGraphMenuItem<int>
    {
        public RecieveGraphMenuItem(IMessenger messenger,
            IInput<int> input, IPathfindingHistory history, 
            IPathfindingRangeBuilder<Vertex> rangeBuilder, 
            ISerializer<SerializationInfo> serializer, 
            ILog log) 
            : base(messenger, input, history, rangeBuilder, serializer, log)
        {
        }

        public override string ToString()
        {
            return Languages.RecieveGraph;
        }

        protected override int InputPath()
        {
            using (Cursor.UseCurrentPositionWithClean())
            {
                return input.Input(Languages.InputPort);
            }
        }

        protected override SerializationInfo ImportGraph(int port)
        {
            System.Console.Write(Languages.WaitingForConnection);
            return serializer.DeserializeFromNetwork(port);
        }
    }
}
