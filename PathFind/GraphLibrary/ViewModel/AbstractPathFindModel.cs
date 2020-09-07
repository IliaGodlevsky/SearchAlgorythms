﻿using GraphLibrary.Algorithm;
using GraphLibrary.AlgorithmEnum;
using GraphLibrary.Collection;
using GraphLibrary.AlgoSelector;
using GraphLibrary.Common.Extensions;
using System.Diagnostics;
using System.Linq;
using GraphLibrary.Extensions;

namespace GraphLibrary.Model
{
    public abstract class AbstractPathFindModel : IModel
    {   
        public int DelayTime { get; set; } // miliseconds
        public Algorithms Algorithm { get; set; }

        public AbstractPathFindModel(IMainModel mainViewModel)
        {
            this.mainViewModel = mainViewModel;
            graph = mainViewModel.Graph;
            pathFindStatisticsFormat = LibraryResources.StatisticsFormat;
            badResultMessage = LibraryResources.BadResultMsg;
            DelayTime = 4;
            timer = new Stopwatch();
        }

        public virtual void FindPath()
        {
            pathAlgorithm = AlgorithmSelector.GetPathFindAlgorithm(Algorithm, graph);
            PrepareAlgorithm();
            var path = pathAlgorithm.FindDestionation();
            mainViewModel.Statistics += string.Format(" " + pathFindStatisticsFormat,
                path.Count(),
                path.Sum(vertex => vertex.Cost),
                graph.NumberOfVisitedVertices);
            mainViewModel.Graph.RemoveExtremeVertices();
        }

        protected IPathFindAlgorithm pathAlgorithm;

        protected virtual void PrepareAlgorithm()
        {
            pathAlgorithm.OnVertexVisited += (vertex) =>
            {
                vertex.IsVisited = true;
                if (vertex.IsSimpleVertex())
                    vertex.MarkAsVisited();
            };
            pathAlgorithm.OnAlgorithmStarted += (sender, eventArgs) => { timer.Start(); };
            pathAlgorithm.OnAlgorithmFinished += (sender, eventArgs) =>
            {
                timer.Stop();
                mainViewModel.Statistics = timer.GetTimeInformation(LibraryResources.TimerInfoFormat);
                pathAlgorithm.DrawPath();

            };
        }

        private readonly Stopwatch timer;
        protected Graph graph;
        protected IMainModel mainViewModel;
        protected string badResultMessage;
        protected string pathFindStatisticsFormat;
    }
}
