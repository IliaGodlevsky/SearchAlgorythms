﻿using Autofac;
using Common.Interface;
using ConsoleVersion.Attributes;
using ConsoleVersion.DependencyInjection;
using ConsoleVersion.Extensions;
using ConsoleVersion.Interface;
using ConsoleVersion.Messages;
using ConsoleVersion.Model;
using GalaSoft.MvvmLight.Messaging;
using GraphLib.Realizations.Graphs;
using System;

namespace ConsoleVersion.ViewModel
{
    internal sealed class VertexStateViewModel : IViewModel, IRequireIntInput, IDisposable
    {
        public event Action WindowClosed;

        private readonly IMessenger messenger;

        private Graph2D Graph { get; set; }

        public IInput<int> IntInput { get; set; }

        private Vertex Vertex => IntInput.InputVertex(Graph);

        public VertexStateViewModel()
        {
            messenger = DI.Container.Resolve<IMessenger>();
            messenger.Register<ClaimGraphAnswer>(this, OnGraphRecieved);
            messenger.Send(new ClaimGraphMessage());
        }

        [MenuItem(MenuItemsNames.ReverseVertex, 0)]
        public void ReverseVertex()
        {
            Vertex.Reverse();
        }

        [MenuItem(MenuItemsNames.ChangeVertexCost, 1)]
        public void ChangeVertexCost()
        {
            Vertex.ChangeCost();
        }

        [MenuItem(MenuItemsNames.Exit, 2)]
        public void Interrupt()
        {
            WindowClosed?.Invoke();
        }

        public void Dispose()
        {
            messenger.Unregister(this);
            WindowClosed = null;
        }

        private void OnGraphRecieved(ClaimGraphAnswer message)
        {
            Graph = (Graph2D)message.Graph;
        }
    }
}
