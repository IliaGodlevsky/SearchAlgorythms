﻿using Algorithm.Algos.Enums;
using Algorithm.Algos.Extensions;
using Algorithm.Extensions;
using Algorithm.Infrastructure.EventArguments;
using Algorithm.Interfaces;
using Algorithm.NullRealizations;
using EnumerationValues.Extensions;
using EnumerationValues.Realizations;
using GraphLib.Base;
using GraphLib.Interfaces;
using GraphViewModel.Interfaces;
using Interruptable.EventArguments;
using Logging.Interface;
using System;
using System.Diagnostics;

namespace GraphViewModel
{
    public abstract class PathFindingModel : IModel
    {
        public bool IsVisualizationRequired { get; set; } = true;

        public int DelayTime { get; set; } = 5;

        public Algorithms Algorithm { get; set; }

        public Tuple<string, Algorithms>[] Algorithms => algorithms.Value;

        protected PathFindingModel(ILog log, IGraph graph,
            BaseEndPoints endPoints)
        {
            this.endPoints = endPoints;
            this.log = log;
            this.graph = graph;
            var enumValues = new EnumValuesWithoutIgnored<Algorithms>(new EnumValues<Algorithms>());
            algorithms = new Lazy<Tuple<string, Algorithms>[]>(enumValues.ToAdjustedAndOrderedByDescriptionTuples);
            timer = new Stopwatch();
            path = new NullGraphPath();
            algorithm = new NullAlgorithm();
        }

        public virtual async void FindPath()
        {
            try
            {
                algorithm = Algorithm.ToAlgorithm(graph, endPoints);
                SubscribeOnAlgorithmEvents(algorithm);
                path = await algorithm.FindPathAsync();
                await path.HighlightAsync(endPoints);
                Summarize();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                algorithm.Dispose();
            }
        }

        protected virtual void OnVertexVisited(object sender, AlgorithmEventArgs e)
        {
            if (CanBeVisualized(e.Current))
            {
                (e.Current as IVisualizable)?.VisualizeAsVisited();
            }
            visitedVerticesCount++;
        }

        protected virtual void OnVertexVisitedNoVisualization(object sender, EventArgs e)
        {
            visitedVerticesCount++;
        }

        protected virtual void OnVertexEnqueued(object sender, AlgorithmEventArgs e)
        {
            if (CanBeVisualized(e.Current))
            {
                (e.Current as IVisualizable)?.VisualizeAsEnqueued();
            }
        }

        protected virtual void OnAlgorithmInterrupted(object sender, ProcessEventArgs e)
        {
            timer.Stop();
        }

        protected virtual void OnAlgorithmFinished(object sender, ProcessEventArgs e)
        {
            timer.Stop();
        }

        protected abstract void Summarize();

        protected virtual void OnAlgorithmStarted(object sender, ProcessEventArgs e)
        {
            timer.Restart();
        }

        protected virtual void SubscribeOnAlgorithmEvents(IAlgorithm algorithm)
        {
            if (IsVisualizationRequired)
            {
                algorithm.VertexEnqueued += OnVertexEnqueued;
                algorithm.VertexVisited += OnVertexVisited;
            }
            else
            {
                algorithm.VertexVisited += OnVertexVisitedNoVisualization;
            }
            algorithm.Finished += OnAlgorithmFinished;
            algorithm.Started += OnAlgorithmStarted;
            algorithm.Interrupted += OnAlgorithmInterrupted;
        }

        private bool CanBeVisualized(IVertex vertex)
        {
            return !endPoints.IsEndPoint(vertex)
                && vertex is IVisualizable;
        }

        protected readonly BaseEndPoints endPoints;
        protected readonly IGraph graph;
        protected readonly ILog log;
        protected readonly Stopwatch timer;

        protected IGraphPath path;
        protected IAlgorithm algorithm;
        protected int visitedVerticesCount;

        private readonly Lazy<Tuple<string, Algorithms>[]> algorithms;
    }
}