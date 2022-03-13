﻿using Autofac;
using Common.Interface;
using ConsoleVersion.Attributes;
using ConsoleVersion.DependencyInjection;
using ConsoleVersion.Enums;
using ConsoleVersion.Extensions;
using ConsoleVersion.Interface;
using ConsoleVersion.Messages;
using ConsoleVersion.Model;
using ConsoleVersion.Views;
using GalaSoft.MvvmLight.Messaging;
using GraphLib.Base.EndPoints;
using GraphLib.Extensions;
using GraphLib.Interfaces;
using GraphLib.Realizations.Graphs;
using GraphLib.Serialization;
using GraphLib.Serialization.Exceptions;
using GraphViewModel;
using GraphViewModel.Interfaces;
using Logging.Interface;
using System;
using System.Drawing;
using static GraphLib.Base.BaseVertexCost;
using Console = Colorful.Console;

namespace ConsoleVersion.ViewModel
{
    internal sealed class MainViewModel : MainModel, IMainModel, IViewModel, IRequireAnswerInput, IRequireIntInput, IDisposable
    {
        public event Action WindowClosed;

        public IInput<int> IntInput { get; set; }
        public IInput<Answer> AnswerInput { get; set; }

        public MainViewModel(IGraphFieldFactory fieldFactory,
            IVertexEventHolder eventHolder, GraphSerializationModule serializationModule, BaseEndPoints endPoints, ILog log)
            : base(fieldFactory, eventHolder, serializationModule, endPoints, log)
        {
            messenger = DI.Container.Resolve<IMessenger>();
            messenger.Register<GraphCreatedMessage>(this, MessageTokens.MainModel, message => ConnectNewGraph(message.Graph));
            messenger.Register<ClearGraphMessage>(this, MessageTokens.MainModel, message => ClearGraph());
            messenger.Register<ClearColorsMessage>(this, MessageTokens.MainModel, message => ClearColors());
            messenger.Register<ClaimGraphMessage>(this, MessageTokens.MainModel, RecieveClaimGraphMessage);
        }

        [MenuItem(MenuItemsNames.MakeUnwieghted, MenuItemPriority.Normal)]
        public void MakeGraphUnweighted() => Graph.ToUnweighted();

        [MenuItem(MenuItemsNames.MakeWeighted, MenuItemPriority.Normal)]
        public void MakeGraphWeighted() => Graph.ToWeighted();

        [MenuItem(MenuItemsNames.CreateNewGraph, MenuItemPriority.Highest)]
        public override void CreateNewGraph() => DI.Container.Display<GraphCreateView>();

        [MenuItem(MenuItemsNames.FindPath, MenuItemPriority.High)]
        public override void FindPath() => DI.Container.Display<PathFindView>();

        [MenuItem(MenuItemsNames.ReverseVertex, MenuItemPriority.Normal)]
        public void ReverseVertex() => ChangeVertexState(vertex => vertex.OnVertexReversed());

        [MenuItem(MenuItemsNames.ChangeVertexCost, MenuItemPriority.Low)]
        public void ChangeVertexCost() => ChangeVertexState(vertex => vertex.OnVertexCostChanged());

        [MenuItem(MenuItemsNames.ChangeCostRange, MenuItemPriority.Low)]
        public void ChangeVertexCostValueRange()
        {
            CostRange = IntInput.InputRange(Constants.VerticesCostRange);
            var message = new CostRangeChangedMessage(CostRange);
            messenger.Forward(message, MessageTokens.MainView);
        }

        [MenuItem(MenuItemsNames.SaveGraph, MenuItemPriority.Normal)]
        public override void SaveGraph() => base.SaveGraph();

        [MenuItem(MenuItemsNames.LoadGraph, MenuItemPriority.Normal)]
        public override void LoadGraph()
        {
            try
            {
                var graph = serializationModule.LoadGraph();
                ConnectNewGraph(graph);
                messenger
                    .Forward(new CostRangeChangedMessage(CostRange), MessageTokens.MainView)
                    .Forward(new GraphCreatedMessage(graph), MessageTokens.MainView);
            }
            catch (CantSerializeGraphException ex)
            {
                log.Warn(ex);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        [MenuItem(MenuItemsNames.Exit, MenuItemPriority.Lowest)]
        public void Interrupt()
        {
            var answer = AnswerInput.Input(MessagesTexts.ExitAppMsg, Constants.AnswerValueRange);
            if (answer == Answer.Yes)
            {
                WindowClosed?.Invoke();
            }
        }

        public override void ClearGraph()
        {
            base.ClearGraph();
            messenger.Forward(UpdateStatisticsMessage.Empty, MessageTokens.MainView);
        }

        public void DisplayGraph()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = Color.White;
                Console.WriteLine(GraphParametres);
                (GraphField as IDisplayable)?.Display();
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

        private void RecieveClaimGraphMessage(ClaimGraphMessage message)
        {
            messenger.Forward(new GraphCreatedMessage(Graph), message.ClaimerMessageToken);
        }

        private void ChangeVertexState(Action<Vertex> changeFunction)
        {
            if (Graph.HasVertices())
            {
                changeFunction(IntInput.InputVertex((Graph2D)Graph));
            }
        }

        public void Dispose()
        {
            messenger.Unregister(this);
            WindowClosed = null;
        }

        private readonly IMessenger messenger;
    }
}