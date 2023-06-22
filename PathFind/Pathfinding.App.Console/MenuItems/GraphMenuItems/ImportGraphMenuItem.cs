﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.DataAccess.Models;
using Pathfinding.App.Console.DataAccess.UnitOfWorks;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Serialization;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.Logging.Interface;
using System;
using System.Linq;

namespace Pathfinding.App.Console.MenuItems.GraphMenuItems
{
    internal abstract class ImportGraphMenuItem<TPath> : IMenuItem
    {
        protected readonly IMessenger messenger;
        protected readonly IInput<TPath> input;
        protected readonly IPathfindingRangeBuilder<Vertex> rangeBuilder;
        protected readonly ISerializer<SerializationInfo> serializer;
        protected readonly IUnitOfWork history;
        protected readonly ILog log;

        protected ImportGraphMenuItem(IMessenger messenger, 
            IInput<TPath> input, 
            IUnitOfWork history,
            IPathfindingRangeBuilder<Vertex> rangeBuilder,
            ISerializer<SerializationInfo> serializer, ILog log)
        {
            this.history = history;
            this.rangeBuilder = rangeBuilder;
            this.serializer = serializer;
            this.messenger = messenger;
            this.input = input;
            this.log = log;
        }

        public virtual void Execute()
        {
            try
            {
                var path = InputPath();
                var info = ImportGraph(path);
                var model = SetGraph(info);
                SetPathfindingResults(info, model);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private GraphModel SetGraph(SerializationInfo info)
        {
            var costRange = info.Graph.First().Cost.CostRange;
            messenger.SendData(costRange, Tokens.AppLayout);
            messenger.SendData(info.Graph, Tokens.AppLayout, Tokens.Main, Tokens.Common);
            var pathfindingRange = info.Range.ToList();
            var target = pathfindingRange[pathfindingRange.Count - 1];
            pathfindingRange.RemoveAt(pathfindingRange.Count - 1);
            pathfindingRange.Insert(1, target);
            rangeBuilder.Undo();
            rangeBuilder.Include(pathfindingRange, info.Graph);
            var model = history.AddGraph(info.Graph);
            model.Range = pathfindingRange.ToArray();
            model = history.UpdateGraph(model);
            history.AddGraphInformation(model.Id, info.GraphInformation);
            messenger.SendData(model, Tokens.Common);
            return model;
        }

        private void SetPathfindingResults(SerializationInfo info, GraphModel model)
        {
            for (int i = 0; i < info.Algorithms.Count; i++)
            {
                var algorithm = history.AddAlgorithm(model.Id, info.Algorithms[i]);
                history.AddVisited(algorithm.Id, info.Visited[i]);
                history.AddObstacles(algorithm.Id, info.Obstacles[i]);
                history.AddPath(algorithm.Id, info.Paths[i]);
                history.AddRange(algorithm.Id, info.Ranges[i]);
                history.AddCosts(algorithm.Id, info.Costs[i]);
                history.AddStatistics(algorithm.Id, info.Statistics[i]);
            }
        }

        protected abstract TPath InputPath();

        protected abstract SerializationInfo ImportGraph(TPath path);
    }
}
