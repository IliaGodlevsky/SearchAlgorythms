﻿using Autofac.Features.AttributeFilters;
using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.ConsoleApp.Injection;
using Pathfinding.ConsoleApp.Model;
using Pathfinding.Domain.Core;
using Pathfinding.Infrastructure.Business.Algorithms;
using Pathfinding.Infrastructure.Business.Extensions;
using Pathfinding.Logging.Interface;
using Pathfinding.Service.Interface;
using Pathfinding.Service.Interface.Requests.Create;
using ReactiveUI;
using System;
using System.Collections.Generic;

namespace Pathfinding.ConsoleApp.ViewModel
{
    internal sealed class AStarGreedyAlgorithmViewModel : PathfindingProcessViewModel,
        IRequireHeuristicsViewModel, IRequireStepRuleViewModel
    {
        private (string Name, IHeuristic Heuristic) heuristic;
        public (string Name, IHeuristic Heuristic) Heuristic
        {
            get => heuristic;
            set => this.RaiseAndSetIfChanged(ref heuristic, value);
        }

        private (string Name, IStepRule StepRule) stepRule;
        public (string Name, IStepRule Rule) StepRule
        {
            get => stepRule;
            set => this.RaiseAndSetIfChanged(ref stepRule, value);
        }

        private double weight;
        public double Weight
        {
            get => weight;
            set => this.RaiseAndSetIfChanged(ref weight, value);
        }

        public AStarGreedyAlgorithmViewModel(
            IRequestService<GraphVertexModel> service,
            [KeyFilter(KeyFilters.ViewModels)] IMessenger messenger,
            ILog logger)
            : base(service, messenger, logger)
        {
        }

        public override string AlgorithmName { get; } = AlgorithmNames.AStarGreedy;

        protected override IObservable<bool> CanStartAlgorithm()
        {
            return this.WhenAnyValue(
                x => x.Graph.Graph,
                x => x.Graph.Id,
                x => x.Weight,
                (graph, id, weight) => id > 0 && graph != null && weight > 0);
        }

        protected override void AppendStatistics(CreateStatisticsRequest model)
        {
            model.StepRule = stepRule.Name;
            model.Heuristics = heuristic.Name;
            model.Weight = Weight;
        }

        protected override PathfindingProcess GetAlgorithm(IEnumerable<GraphVertexModel> pathfindingRange)
        {
            return new AStarGreedyAlgorithm(pathfindingRange,
                heuristic.Heuristic.WithWeight(Weight), stepRule.StepRule);
        }
    }
}
