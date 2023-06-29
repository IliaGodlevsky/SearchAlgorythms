﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.AlgorithmLib.Core.Abstractions;
using Pathfinding.AlgorithmLib.Core.Events;
using Pathfinding.AlgorithmLib.Core.Exceptions;
using Pathfinding.AlgorithmLib.Core.NullObjects;
using Pathfinding.AlgorithmLib.Factory.Interface;
using Pathfinding.App.Console.DataAccess;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.Logging.Interface;
using Pathfinding.Visualization.Extensions;
using Shared.Extensions;
using Shared.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.App.Console.Units
{
    internal sealed class PathfindingProcessUnit : Unit, ICanRecieveMessage
    {
        private readonly IMessenger messenger;
        private readonly IPathfindingRangeBuilder<Vertex> rangeBuilder;
        private readonly IInput<ConsoleKey> input;
        private readonly ILog log;
        private readonly PathfindingHistory history;

        private Graph2D<Vertex> graph = Graph2D<Vertex>.Empty;

        public PathfindingProcessUnit(IReadOnlyCollection<IMenuItem> menuItems,
            IReadOnlyCollection<IConditionedMenuItem> conditioned,
            IPathfindingRangeBuilder<Vertex> rangeBuilder,
            IInput<ConsoleKey> input,
            IMessenger messenger,
            PathfindingHistory history,
            ILog log)
            : base(menuItems, conditioned)
        {
            this.history = history;
            this.messenger = messenger;
            this.log = log;
            this.rangeBuilder = rangeBuilder;
            this.input = input;
        }

        private void FindPath(IAlgorithmFactory<PathfindingProcess> factory)
        {
            var range = rangeBuilder.Range;
            using (var algorithm = factory.Create(range))
            {
                using (Disposable.Use(ClearColors))
                {
                    using (Cursor.HideCursor())
                    {
                        try
                        {
                            FindPath(algorithm);
                        }
                        catch (PathfindingException ex)
                        {
                            log.Warn(ex.Message);
                        }
                    }
                }
            }
        }

        private void ClearColors()
        {
            graph.RestoreVerticesVisualState();
            rangeBuilder.Range.RestoreVerticesVisualState();
            messenger.SendData(string.Empty, Tokens.AppLayout);
        }

        private void FindPath(PathfindingProcess algorithm)
        {
            var path = NullGraphPath.Interface;
            void Summarize()
            {
                messenger.SendData((algorithm, path), Tokens.Statistics);
                history.GetFor(graph).Paths.TryGetOrAddNew(algorithm.Id).AddRange(path);
            }
            using (Disposable.Use(Summarize))
            {
                PrepareForPathfinding(algorithm);
                path = algorithm.FindPath();
                path.Select(graph.Get).ForEach(v => v.VisualizeAsPath());
            }
            input.Input();
        }

        private void SetGraph(Graph2D<Vertex> graph)
        {
            this.graph = graph;
        }

        private void OnVertexVisited(object sender, PathfindingEventArgs e)
        {
            if (sender is PathfindingProcess process)
            {
                var visited = history.GetFor(graph).Visited;
                visited.TryGetOrAddNew(process.Id).Add(e.Current);
            }
        }

        private void PrepareForPathfinding(PathfindingProcess algorithm)
        {
            var hist = history.GetFor(graph);
            hist.Algorithms.Add(algorithm.Id);
            hist.Obstacles.TryGetOrAddNew(algorithm.Id).AddRange(graph.GetObstaclesCoordinates());
            hist.Costs.TryGetOrAddNew(algorithm.Id).AddRange(graph.GetCosts());
            hist.Ranges.TryGetOrAddNew(algorithm.Id).AddRange(rangeBuilder.Range.GetCoordinates());
            algorithm.VertexVisited += OnVertexVisited;
            messenger.SendData(algorithm.ToString(), Tokens.AppLayout);
            messenger.SendData(algorithm, Tokens.Visualization, Tokens.Statistics);
        }

        public void RegisterHanlders(IMessenger messenger)
        {
            messenger.RegisterData<IAlgorithmFactory<PathfindingProcess>>(this, Tokens.Pathfinding, FindPath);
            messenger.RegisterGraph(this, Tokens.Common, SetGraph);
            messenger.Register<ClearColorsMessage>(this, _ => ClearColors());
        }
    }
}