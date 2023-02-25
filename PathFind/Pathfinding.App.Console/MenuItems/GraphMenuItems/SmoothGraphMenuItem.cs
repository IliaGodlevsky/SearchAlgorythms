﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.GraphLib.Smoothing;
using Pathfinding.GraphLib.Smoothing.Interface;
using System.Collections.Generic;

namespace Pathfinding.App.Console.MenuItems.GraphMenuItems
{
    [LowPriority]
    internal sealed class SmoothGraphMenuItem : IConditionedMenuItem, ICanRecieveMessage
    {
        private readonly IMeanCost meanAlgorithm;
        private readonly IMessenger messenger;
        private readonly IInput<int> input;

        private Graph2D<Vertex> graph = Graph2D<Vertex>.Empty;

        private IReadOnlyList<ISmoothLevel> SmoothLevels => ConsoleSmoothLevels.Levels;

        public SmoothGraphMenuItem(IMeanCost meanAlgorithm, IMessenger messenger, IInput<int> input)
        {
            this.meanAlgorithm = meanAlgorithm;
            this.messenger = messenger;
            this.input = input;
        }

        public bool CanBeExecuted() => graph != Graph2D<Vertex>.Empty;

        public void Execute()
        {
            var menuList = SmoothLevels.CreateMenuList();
            var message = menuList + "\n" + Languages.SmoothLevelMsg;
            using (Cursor.UseCurrentPositionWithClean())
            {
                int index = input.Input(message, SmoothLevels.Count, 1) - 1;
                var level = SmoothLevels[index];
                graph.Smooth(meanAlgorithm, level.Level);
            }
        }

        private void OnGraphCreated(GraphCreatedMessage message)
        {
            graph = message.Graph;
        }

        public override string ToString()
        {
            return Languages.SmoothGraph;
        }

        public void RegisterHanlders(IMessenger messenger)
        {
            messenger.Register<GraphCreatedMessage>(this, OnGraphCreated);
        }
    }
}