﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.AlgorithmLib.Core.Abstractions;
using Pathfinding.AlgorithmLib.Core.Events;
using Pathfinding.AlgorithmLib.Core.Interface;
using Pathfinding.AlgorithmLib.History.Interface;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Interface;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.Visualization.Extensions;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.App.Console.Units
{
    internal sealed class PathfindingHistoryUnit : Unit, ICanRecieveMessage
    {
        private readonly Dictionary<Guid, List<ICoordinate>> visited = new();        
        private readonly IPathfindingHistory history;

        private Graph2D<Vertex> graph = Graph2D<Vertex>.Empty;
        private bool isHistoryApplied = true;

        public PathfindingHistoryUnit(IReadOnlyCollection<IMenuItem> menuItems,
            IReadOnlyCollection<IConditionedMenuItem> conditioned,
            IPathfindingHistory history)
            : base(menuItems, conditioned)
        {
            this.history = history;
        }

        private void VisualizeHistory(Guid key)
        {
            graph.RestoreVerticesVisualState();
            graph.ApplyCosts(history.Costs.Get(key));
            history.ObstacleVertices.Get(key).Select(graph.Get)
                .ForEach(vertex => vertex.VisualizeAsObstacle());
            history.VisitedVertices.Get(key).Select(graph.Get)
                .ForEach(vertex=>vertex.VisualizeAsVisited());
            history.RangeVertices.Get(key).Select(graph.Get).Reverse().VisualizeAsRange();
            history.PathVertices.Get(key).Select(graph.Get).VisualizeAsPath();
        }

        private void SetIsApplied(bool isApplied)
        {
            isHistoryApplied = isApplied;
        }

        private void SetGraph(Graph2D<Vertex> graph)
        {
            this.graph = graph;
            ClearHistory(null);
        }

        private void ClearHistory(ClearHistoryMessage msg)
        {
            history.ObstacleVertices.RemoveAll();
            history.PathVertices.RemoveAll();
            history.RangeVertices.RemoveAll();
            history.Costs.RemoveAll();
            history.VisitedVertices.RemoveAll();
            history.Algorithms.Clear();
        }

        private void OnVertexVisited(object sender, PathfindingEventArgs e)
        {
            if (sender is IHistoryPageKey key)
            {
                visited.TryGetOrAddNew(key.Id).Add(e.Current);
            }
        }

        private void OnAlgorithmStarted(object sender, EventArgs e)
        {
            if (sender is PathfindingProcess process)
            {
                history.Algorithms.Add(process.Id);
            }
        }

        private bool IsHistoryApplied() => isHistoryApplied;

        private void AddRange((PathfindingProcess Algorithm, IPathfindingRange<Vertex> Range) value)
        {
            var range = value.Range.GetCoordinates().ToList();
            history.RangeVertices.Add(value.Algorithm.Id, range);
        }

        private void AddPath((PathfindingProcess Algorithm, IGraphPath Path) value)
        {
            history.PathVertices.Add(value.Algorithm.Id, value.Path.ToList());
        }

        private void SubscribeOnHistory(PathfindingProcess algorithm)
        {
            var obstacles = graph.GetObstaclesCoordinates().ToList();
            history.ObstacleVertices.Add(algorithm.Id, obstacles);
            history.Costs.Add(algorithm.Id, graph.GetCosts());
            algorithm.VertexVisited += OnVertexVisited;
            algorithm.Finished += OnAlgorithmFinished;
            algorithm.Started += OnAlgorithmStarted;
        }

        private void OnAlgorithmFinished(object sender, EventArgs e)
        {
            if (sender is PathfindingProcess process)
            {
                history.VisitedVertices.Add(process.Id, visited[process.Id]);
                visited.Remove(process.Id);
            }
        }

        public void RegisterHanlders(IMessenger messenger)
        {
            var token = Tokens.Bind(IsHistoryApplied, Tokens.History);
            messenger.RegisterAlgorithmData<IPathfindingRange<Vertex>>(this, token, AddRange);
            messenger.RegisterAlgorithmData<IGraphPath>(this, token, AddPath);
            messenger.RegisterData<PathfindingProcess>(this, token, SubscribeOnHistory);
            messenger.RegisterGraph(this, Tokens.Common, SetGraph);
            messenger.RegisterData<bool>(this, Tokens.History, SetIsApplied);
            messenger.RegisterData<Guid>(this, token, VisualizeHistory);
            messenger.Register<ClearHistoryMessage>(this, Tokens.History, ClearHistory);
        }
    }
}