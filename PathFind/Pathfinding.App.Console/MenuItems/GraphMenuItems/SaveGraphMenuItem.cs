﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Serialization;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Extensions;
using Pathfinding.Logging.Interface;
using System.Threading.Tasks;

namespace Pathfinding.App.Console.MenuItems.GraphMenuItems
{
    [LowPriority]
    internal sealed class SaveGraphMenuItem : ExportGraphMenuItem<string>
    {
        public SaveGraphMenuItem(IMessenger messenger, 
            IFilePathInput input, IPathfindingHistory history, 
            ISerializer<SerializationInfo> graphSerializer, 
            IPathfindingRangeBuilder<Vertex> rangeBuilder, 
            ILog log) 
            : base(messenger, input, history, graphSerializer, rangeBuilder, log)
        {
        }

        public override string ToString() => Languages.SaveGraph;


        protected override async Task ExportAsync(SerializationInfo info, string path)
        {
            await graphSerializer.SerializeToFileAsync(info, path);
        }
    }
}
