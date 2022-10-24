﻿using Algorithm.Base;
using Algorithm.Factory.Interface;
using Algorithm.Infrastructure.EventArguments;
using Autofac;
using Common.Extensions;
using Common.Interface;
using GalaSoft.MvvmLight.Messaging;
using GraphLib.Base.EndPoints;
using GraphViewModel;
using Interruptable.EventArguments;
using Logging.Interface;
using System;
using System.Collections.Generic;
using WindowsFormsVersion.DependencyInjection;
using WindowsFormsVersion.Enums;
using WindowsFormsVersion.Messeges;

namespace WindowsFormsVersion.ViewModel
{
    internal class PathFindingViewModel : PathFindingModel, IViewModel, IDisposable
    {
        public event Action WindowClosed;

        public PathFindingViewModel(BasePathfindingRange endPoints,
            IEnumerable<IAlgorithmFactory<PathfindingAlgorithm>> algorithmFactories, ILog log)
            : base(endPoints, algorithmFactories, log)
        {
            Delay = Constants.AlgorithmDelayTimeValueRange.LowerValueOfRange;
            messenger = DI.Container.Resolve<IMessenger>();
        }

        protected override void OnAlgorithmFinished(object sender, ProcessEventArgs e)
        {
            messenger.Send(AlgorithmStatusMessage.Finished, MessageTokens.MainModel);
        }

        protected override void OnAlgorithmStarted(object sender, ProcessEventArgs e)
        {
            messenger.Send(AlgorithmStatusMessage.Started, MessageTokens.MainModel);
        }

        protected override void SummarizePathfindingResults()
        {
            string statistics = path.Count > 0 ? Statistics : CouldntFindPath;
            var message = new UpdateStatisticsMessage(statistics);
            messenger.Send(message, MessageTokens.MainModel);
        }

        protected override void OnVertexVisited(object sender, AlgorithmEventArgs e)
        {
            Delay.Wait();
            base.OnVertexVisited(sender, e);
            var message = new UpdateStatisticsMessage(Statistics);
            messenger.Send(message, MessageTokens.MainModel);
        }

        protected override void OnAlgorithmInterrupted(object sender, ProcessEventArgs e)
        {
            messenger.Send(AlgorithmStatusMessage.Finished, MessageTokens.MainModel);
        }

        public void StartPathfinding(object sender, EventArgs e)
        {
            if (CanExecuteConfirmPathFindAlgorithmChoice())
            {
                WindowClosed?.Invoke();
                FindPath();
            }
        }

        public void CancelPathFinding(object sender, EventArgs e)
        {
            WindowClosed?.Invoke();
        }

        private bool CanExecuteConfirmPathFindAlgorithmChoice()
        {
            return Algorithm != null;
        }

        public void Dispose()
        {
            WindowClosed = null;
        }

        private string Statistics
        {
            get
            {
                string timerInfo = timer.Elapsed.ToString(@"mm\:ss\.fff");
                string description = Algorithm.ToString();
                string pathfindingInfo = string.Format(Format, PathfindingInfo);
                return string.Join("    ", description, timerInfo, pathfindingInfo);
            }
        }

        private object[] PathfindingInfo => new object[] { path.Count, path.Cost, visitedVerticesCount };

        private readonly string Format = "Steps: {0}   Path cost: {1}   Visited: {2}";
        private readonly string CouldntFindPath = "Could't fing path";

        private readonly IMessenger messenger;
    }
}