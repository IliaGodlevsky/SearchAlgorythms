﻿using Autofac;
using Common.Interface;
using ConsoleVersion.Attributes;
using ConsoleVersion.DependencyInjection;
using ConsoleVersion.Enums;
using ConsoleVersion.Extensions;
using ConsoleVersion.Interface;
using ConsoleVersion.Messages;
using ConsoleVersion.Views;
using GalaSoft.MvvmLight.Messaging;
using GraphLib.Base.EndPoints;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using GraphLib.NullRealizations;
using Logging.Interface;
using NullObject.Extensions;
using System;
using System.Drawing;
using static GraphLib.Base.BaseVertexCost;
using Console = Colorful.Console;

namespace ConsoleVersion.ViewModel
{
    internal sealed class MainViewModel : IViewModel, IRequireAnswerInput, IRequireIntInput, IDisposable
    {
        public event Action WindowClosed;

        private readonly ILog log;
        private readonly IMessenger messenger;
        private readonly IVertexEventHolder eventHolder;       
        private readonly IGraphFieldFactory fieldFactory;
        private readonly BaseEndPoints endPoints;

        private IGraph graph;
        private IGraphField graphField;

        public string GraphParamters { get; set; }

        public IInput<int> IntInput { get; set; }

        public IInput<Answer> AnswerInput { get; set; }

        public MainViewModel(IGraphFieldFactory fieldFactory, IVertexEventHolder eventHolder, BaseEndPoints endPoints, ILog log)
        {
            graph = NullGraph.Instance;
            messenger = DI.Container.Resolve<IMessenger>();
            messenger.Register<GraphCreatedMessage>(this, SetGraph);
            messenger.Register<ClearGraphMessage>(this, message => ClearGraph());
            messenger.Register<ClearColorsMessage>(this, message => ClearColors());
            messenger.Register<ClaimGraphMessage>(this, RecieveClaimGraphMessage);
            this.fieldFactory = fieldFactory;
            this.eventHolder = eventHolder;
            this.endPoints = endPoints;
            this.log = log;
        }

        [MenuItem(MenuItemsNames.CreateNewGraph, 0)]
        private void CreateNewGraph()
        {
            DI.Container.Display<GraphCreateView>();
        }

        [PreValidationMethod(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.FindPath, 1)]
        private void FindPath()
        {
            DI.Container.Display<PathFindView>();
        }

        [PreValidationMethod(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.ChangedVertexState, 4)]
        private void ChangeVertexState()
        {
            DI.Container.Display<VertexStateView>();
        }

        [ExecuteSafe(nameof(ExecuteSafe))]
        [MenuItem(MenuItemsNames.ChangeCostRange, 7)]
        private void ChangeVertexCostValueRange()
        {
            CostRange = IntInput.InputRange(Constants.VerticesCostRange);
            messenger.Send(new CostRangeChangedMessage(CostRange));
        }

        [PreValidationMethod(nameof(IsGraphValid))]
        [MenuItem(MenuItemsNames.SaveGraph, 5)]
        private void SaveGraph()
        {
            DI.Container.Display<GraphSaveView>();
        }

        [ExecuteSafe(nameof(ExecuteSafe))]
        [MenuItem(MenuItemsNames.LoadGraph, 6)]
        private void LoadGraph()
        {
            DI.Container.Display<GraphLoadView>();
        }

        [PreValidationMethod(nameof(CanExecuteInterrupt))]
        [MenuItem(MenuItemsNames.Exit, 8)]
        private void Interrupt()
        {
            WindowClosed?.Invoke();
        }

        public void ClearGraph()
        {
            graph.Refresh();
            GraphParamters = graph.ToString();
            endPoints.Reset();
            messenger.Send(UpdateStatisticsMessage.Empty);
        }

        public void ClearColors()
        {
            graph.Refresh();
            endPoints.RestoreCurrentColors();
        }

        public void DisplayGraph()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = Color.White;
                Console.WriteLine(GraphParamters);
                (graphField as IDisplayable)?.Display();
                Console.WriteLine();
                MainView.SetCursorPositionUnderMenu(1);
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
            endPoints.UnsubscribeFromEvents(graph);
            endPoints.Reset();
            eventHolder.UnsubscribeVertices(graph);
            graph = message.Graph;
            graphField = fieldFactory.CreateGraphField(graph);
            endPoints.SubscribeToEvents(graph);
            eventHolder.SubscribeVertices(graph);
            GraphParamters = graph.ToString();
        }

        private void RecieveClaimGraphMessage(ClaimGraphMessage message)
        {
            if (!graph.IsNull())
            {
                messenger.Send(new ClaimGraphAnswer(graph));
            }
        }

        private bool IsGraphValid()
        {
            return !graph.IsNull() && graph.HasVertices();
        }

        private bool CanExecuteInterrupt()
        {
            return AnswerInput.Input(MessagesTexts.ExitAppMsg, Constants.AnswerValueRange) == Answer.Yes;
        }

        private void ExecuteSafe(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public void Dispose()
        {
            messenger.Unregister(this);
            WindowClosed = null;
        }
    }
}