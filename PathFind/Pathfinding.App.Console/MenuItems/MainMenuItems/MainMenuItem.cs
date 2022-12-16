﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Messages;
using Pathfinding.App.Console.Model;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.Logging.Interface;

namespace Pathfinding.App.Console.MenuItems.MainMenuItems
{
    internal abstract class MainMenuItem<TViewModel> : UnitDisplayMenuItem<TViewModel>
        where TViewModel : IUnit
    {
        private readonly IMessenger messenger;

        private Graph2D<Vertex> graph = Graph2D<Vertex>.Empty;

        protected MainMenuItem(IViewFactory viewFactory, TViewModel viewModel, IMessenger messenger, ILog log) 
            : base(viewFactory, viewModel, log)
        {
            this.messenger = messenger;
            this.messenger.Register<GraphCreatedMessage>(this, OnGraphCreated);
        }

        public override bool CanBeExecuted()
        {
            return graph != Graph2D<Vertex>.Empty;
        }

        private void OnGraphCreated(GraphCreatedMessage msg)
        {
            graph = msg.Graph;
        }
    }
}