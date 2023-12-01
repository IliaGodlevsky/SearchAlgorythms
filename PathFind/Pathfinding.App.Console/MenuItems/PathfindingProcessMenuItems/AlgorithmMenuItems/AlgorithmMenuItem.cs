﻿using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.AlgorithmLib.Core.Abstractions;
using Pathfinding.AlgorithmLib.Factory.Interface;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model.Notes;

namespace Pathfinding.App.Console.MenuItems.PathfindingProcessMenuItems.AlgorithmMenuItems
{
    internal abstract class AlgorithmMenuItem : IMenuItem
    {
        protected readonly struct AlgorithmInfo
        {
            public readonly IAlgorithmFactory<PathfindingProcess> Factory { get; }

            public readonly Statistics InitStatistics { get; }

            public AlgorithmInfo(IAlgorithmFactory<PathfindingProcess> factory,
                Statistics initStatistics)
            {
                Factory = factory;
                InitStatistics = initStatistics;
            }
        }

        protected readonly IMessenger messenger;

        protected AlgorithmMenuItem(IMessenger messenger)
        {
            this.messenger = messenger;
        }

        public virtual void Execute()
        {
            var info = GetAlgorithm();
            var msg = new AlgorithmStartInfoMessage(info.Factory, info.InitStatistics);
            messenger.Send(msg, Tokens.Pathfinding);
        }

        protected abstract AlgorithmInfo GetAlgorithm();
    }
}
