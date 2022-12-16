﻿using Autofac;
using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.WPF._2D.Infrastructure;
using Pathfinding.App.WPF._2D.Interface;
using Pathfinding.App.WPF._2D.Messages.DataMessages;
using Pathfinding.App.WPF._2D.Model;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.GraphLib.Factory.Extensions;
using Pathfinding.GraphLib.Factory.Interface;
using Pathfinding.GraphLib.Factory.Realizations.Layers;
using Pathfinding.Logging.Interface;
using Shared.Primitives.Extensions;
using Shared.Primitives.ValueRange;
using Shared.Random;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using WPFVersion.DependencyInjection;

namespace Pathfinding.App.WPF._2D.ViewModel
{
    public class GraphCreatingViewModel : IViewModel, IDisposable
    {
        public event Action WindowClosed;

        private readonly ILog log;
        private readonly IMessenger messenger;
        private readonly IRandom random;
        private readonly INeighborhoodFactory neighborhoodFactory;
        private readonly IVertexCostFactory costFactory;

        public ICommand ConfirmCreateGraphCommand { get; }

        public ICommand CancelCreateGraphCommand { get; }

        public int Width { get; set; }

        public int Length { get; set; }

        public int ObstaclePercent { get; set; }

        public int UpperValueOfRange { get; set; } = 9;

        private InclusiveValueRange<int> CostRange { get; set; }

        public IReadOnlyList<IGraphAssemble<Graph2D<Vertex>, Vertex>> GraphAssembles { get; set; }

        public IGraphAssemble<Graph2D<Vertex>, Vertex> SelectedGraphAssemble { get; set; }

        public GraphCreatingViewModel(ILog log, IRandom random, INeighborhoodFactory neighborhoodFactory, 
            IVertexCostFactory costFactory)
        {
            this.log = log;
            this.messenger = DI.Container.Resolve<IMessenger>();
            ConfirmCreateGraphCommand = new RelayCommand(ExecuteConfirmCreateGraphCommand, CanExecuteConfirmCreateGraphCommand);
            CancelCreateGraphCommand = new RelayCommand(ExecuteCloseWindowCommand);
            this.neighborhoodFactory = neighborhoodFactory;
            this.costFactory = costFactory;
            this.random = random;
        }

        private async void CreateGraph()
        {
            try
            {
                CostRange = new InclusiveValueRange<int>(UpperValueOfRange, 1);
                var layers = GetLayers();
                var graph = await SelectedGraphAssemble.AssembleGraphAsync(layers, Width, Length);
                messenger.Send(new GraphCreatedMessage(graph));
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void ExecuteConfirmCreateGraphCommand(object param)
        {
            CreateGraph();
            ExecuteCloseWindowCommand(null);
        }

        private void ExecuteCloseWindowCommand(object param)
        {
            WindowClosed?.Invoke();
        }

        private bool CanExecuteConfirmCreateGraphCommand(object sender)
        {
            return SelectedGraphAssemble != null
                && Constants.GraphWidthValueRange.Contains(Width)
                && Constants.GraphLengthValueRange.Contains(Length);
        }

        private IReadOnlyCollection<ILayer<Graph2D<Vertex>, Vertex>> GetLayers()
        {
            return new ILayer<Graph2D<Vertex>, Vertex>[]
            {
                new ObstacleLayer<Graph2D<Vertex>, Vertex>(random, ObstaclePercent),
                new VertexCostLayer<Graph2D<Vertex>,Vertex>(costFactory, CostRange, random),
                new NeighborhoodLayer<Graph2D<Vertex>, Vertex>(neighborhoodFactory)
            };
        }

        public void Dispose()
        {
            WindowClosed = null;
        }
    }
}