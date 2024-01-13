﻿using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.App.Console.DAL.Interface;
using Pathfinding.App.Console.DAL.Models.TransferObjects;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Messaging;
using Pathfinding.App.Console.Messaging.Messages;
using Pathfinding.App.Console.Settings;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Smoothing;
using Pathfinding.GraphLib.Smoothing.Interface;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.App.Console.MenuItems.GraphMenuItems
{
    [HighestPriority]
    [Obsolete("Has no point")]
    internal sealed class SmoothGraphMenuItem : IConditionedMenuItem, ICanRecieveMessage
    {
        private readonly IMeanCost meanAlgorithm;
        private readonly IInput<ConsoleKey> input;
        private readonly Dictionary<int, Stack<IReadOnlyList<int>>> smoothes = new();
        private readonly Dictionary<ConsoleKey, Action> actions;
        private readonly IService service;

        private Stack<IReadOnlyList<int>> SmoothHistory
            => smoothes.TryGetOrAddNew(GraphDto.Id);

        private GraphReadDto GraphDto = GraphReadDto.Empty;

        public SmoothGraphMenuItem(IMeanCost meanAlgorithm,
            IInput<ConsoleKey> input,
            IService service)
        {
            this.meanAlgorithm = meanAlgorithm;
            this.input = input;
            this.service = service;
            actions = new Dictionary<ConsoleKey, Action>()
            {
                { Keys.Default.SmoothGraph, Smooth },
                { Keys.Default.UndoSmooth, Undo },
                { Keys.Default.ResetSmooth, Cancel }
            };
        }

        public bool CanBeExecuted() => GraphDto != GraphReadDto.Empty;

        public void Execute()
        {
            using (Cursor.HideCursor())
            {
                var key = input.Input();
                while (key != Keys.Default.SubmitSmooth)
                {
                    actions.GetOrDefault(key)?.Invoke();
                    key = input.Input();
                }
                if (SmoothHistory.Count > 0)
                {
                    service.UpdateVertices(GraphDto.Graph, GraphDto.Id);
                }
            }
        }

        private void Smooth()
        {
            var graph = GraphDto.Graph;
            SmoothHistory.Push(graph.GetCosts());
            graph.Smooth(meanAlgorithm);
            graph.ForEach(v => v.Display());
        }

        private void Undo()
        {
            var graph = GraphDto.Graph;
            if (SmoothHistory.Count > 0)
            {
                graph.Reverse().ApplyCosts(SmoothHistory.Pop().Reverse());
                graph.ForEach(v => v.Display());
            }
        }

        private void Cancel()
        {
            var graph = GraphDto.Graph;
            if (SmoothHistory.Count > 0)
            {
                var initPrices = SmoothHistory.Last().Reverse();
                graph.Reverse().ApplyCosts(initPrices);
                graph.ForEach(v => v.Display());
                SmoothHistory.Clear();
            }
        }

        private void OnGraphCreated(GraphMessage msg)
        {
            GraphDto = msg.Graph;
        }

        public override string ToString()
        {
            return Languages.SmoothGraphItem;
        }

        public void RegisterHanlders(IMessenger messenger)
        {
            messenger.RegisterGraph(this, Tokens.Common, OnGraphCreated);
        }
    }
}