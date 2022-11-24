﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.ConvertedProperties;
using Pathfinding.App.Console.DependencyInjection;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Model.Menu.Attributes;
using Pathfinding.App.Console.Views;
using Pathfinding.GraphLib.Core.Realizations;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.Logging.Interface;
using Pathfinding.Visualization.Core.Abstractions;
using Pathfinding.Visualization.Extensions;
using Pathfinding.VisualizationLib.Core.Interface;
using System;
using System.Drawing;

namespace Pathfinding.App.Console.ViewModel
{
    [MenuColumnsNumber(3)]
    internal sealed class MainViewModel : SafeViewModel, ICache<Graph2D<Vertex>>, IRequireAnswerInput, IRequireIntInput
    {
        private readonly IMessenger messenger;
        private readonly IGraphFieldFactory<Graph2D<Vertex>, Vertex, GraphField> fieldFactory;
        private readonly VisualPathfindingRange<Vertex> adapter;

        private GraphField GraphField { get; set; } = GraphField.Empty;

        private IProperty<string> GraphParamters { get; set; }

        public IInput<int> IntInput { get; set; }

        public IInput<Answer> AnswerInput { get; set; }

        public Graph2D<Vertex> Cached { get; private set; } = Graph2D<Vertex>.Empty;

        public MainViewModel(IGraphFieldFactory<Graph2D<Vertex>, Vertex, GraphField> fieldFactory,
            VisualPathfindingRange<Vertex> adapter, IMessenger messenger, ILog log)
            : base(log)
        {
            Cached = Graph2D<Vertex>.Empty;
            this.messenger = messenger;
            messenger.Register<GraphCreatedMessage>(this, MessageTokens.MainViewModel, SetGraph);
            messenger.Register<ClearGraphMessage>(this, ClearGraph);
            this.fieldFactory = fieldFactory;
            this.adapter = adapter;
        }

        [ExecuteSafe(nameof(ExecuteSafe))]
        [Condition(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.FindPath, 1)]
        private void FindPath() => DI.Container.Display<PathfindingView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [MenuItem(MenuItemsNames.CreateNewGraph, 0)]
        private void CreateNewGraph() => DI.Container.Display<GraphCreateView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [Condition(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.SmoothGraph, 2)]
        private void SmoothGraph() => DI.Container.Display<GraphSmoothView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [Condition(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.ChangedVertexState, 3)]
        private void ChangeVertexState() => DI.Container.Display<VertexStateView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [Condition(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.SaveGraph, 4)]
        private void SaveGraph() => DI.Container.Display<GraphSaveView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [MenuItem(MenuItemsNames.LoadGraph, 5)]
        private void LoadGraph() => DI.Container.Display<GraphLoadView>();

        [ExecuteSafe(nameof(ExecuteSafe))]
        [MenuItem(MenuItemsNames.ChangeCostRange, 6)]
        private void ChangeVertexCostValueRange()
        {
            using (Cursor.CleanUpAfter())
            {
                VertexCost.CostRange = IntInput.InputRange(Constants.VerticesCostRange);
                messenger.Send(new CostRangeChangedMessage(VertexCost.CostRange));
            }
        }

        protected override void RaiseViewClosed()
        {
            using (Cursor.CleanUpAfter())
            {
                if (AnswerInput.Input(MessagesTexts.ExitAppMsg, Answer.Range))
                {
                    base.RaiseViewClosed();
                }
            }
        }

        private void ClearGraph(ClearGraphMessage message)
        {
            Cached.RestoreVerticesVisualState();
            adapter.Undo();
            messenger.Send(UpdatePathfindingStatisticsMessage.Empty);
        }

        private void DisplayGraph()
        {
            try
            {
                System.Console.Clear();
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.WriteLine(GraphParamters);
                GraphField.Display();
            }
            catch (ArgumentOutOfRangeException ex)
            {
                log.Warn(ex);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void SetGraph(GraphCreatedMessage message)
        {
            adapter.Undo();
            Cached = message.Graph;
            GraphField = fieldFactory.CreateGraphField(Cached);
            GraphParamters = GraphParamsProperty.Assign(Cached);
            DisplayGraph();
        }

        [FailMessage(MessagesTexts.GraphIsNotCreatedMsg)]
        private bool IsGraphValid()
        {
            return Cached != Graph2D<Vertex>.Empty;
        }

        public override void Dispose()
        {
            base.Dispose();
            messenger.Unregister(this);
        }
    }
}