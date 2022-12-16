﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.AlgorithmLib.Core.Events;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using System;
using System.Collections.Generic;
using Pathfinding.AlgorithmLib.History.Interface;
using Pathfinding.AlgorithmLib.History;
using Pathfinding.Visualization.Extensions;
using Pathfinding.GraphLib.Core.Interface.Extensions;

namespace Pathfinding.App.Console.ViewModel
{
    internal sealed class PathfindingHistoryUnit : Unit
    {
        private readonly IMessenger messenger;
        private readonly History<PathfindingHistoryVolume> history = new();

        private Graph2D<Vertex> graph = Graph2D<Vertex>.Empty;
        private bool isHistoryApplied = false;

        public PathfindingHistoryUnit(IReadOnlyCollection<IMenuItem> menuItems, IMessenger messenger)
            : base(menuItems)
        {
            this.messenger = messenger;
            this.messenger.Register<PathfindingRangeChosenMessage>(this, OnRangeChosen);
            this.messenger.Register<PathFoundMessage>(this, OnPathFound);
            this.messenger.Register<SubscribeOnHistoryMessage>(this, OnSubscribeOnHistory);
            this.messenger.Register<GraphCreatedMessage>(this, OnGraphCreated);
            this.messenger.Register<ApplyHistoryMessage>(this, OnHistoryApplied);
            this.messenger.Register<HistoryPageMessage>(this, OnHistoryPage);
            this.messenger.Register<ClearHistoryMessage>(this, ClearHistory);
        }

        private void OnHistoryPage(HistoryPageMessage message)
        {
            graph.RestoreVerticesVisualState();
            history.VisualizeHistory(message.PageKey, graph);
        }

        private void OnHistoryApplied(ApplyHistoryMessage message)
        {
            isHistoryApplied = message.IsApplied;
        }

        private void OnGraphCreated(GraphCreatedMessage message)
        {
            graph = message.Graph;
            history.Clear();
        }

        private void ClearHistory(ClearHistoryMessage message)
        {
            history.Clear();
        }

        private void OnVertexVisited(object sender, PathfindingEventArgs e)
        {
            if (sender is IHistoryPageKey key)
            {
                history.AddVisited(key.Id, e.Current);
            }
        }

        private void OnMessageRecieved(Action action) 
        {
            if (isHistoryApplied)
            {
                action();
            }
        }

        private void OnRangeChosen(PathfindingRangeChosenMessage msg)
        {
            OnMessageRecieved(() => history.AddPathfindingRange(msg.Algorithm.Id, msg.Range));
        }

        private void OnPathFound(PathFoundMessage msg)
        {
            OnMessageRecieved(() => history.AddPath(msg.Algorithm.Id, msg.Path));
        }

        private void OnSubscribeOnHistory(SubscribeOnHistoryMessage msg)
        {
            OnMessageRecieved(() => 
            {
                history.AddRegulars(msg.Algorithm.Id, graph.GetNotObstaclesCoordinates());
                history.AddObstacles(msg.Algorithm.Id, graph.GetObstaclesCoordinates());
                msg.Algorithm.VertexVisited += OnVertexVisited; 
            });
        }
    }
}