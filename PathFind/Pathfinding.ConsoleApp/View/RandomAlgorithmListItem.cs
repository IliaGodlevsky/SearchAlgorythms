﻿using Autofac.Features.AttributeFilters;
using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.ConsoleApp.Injection;
using Pathfinding.ConsoleApp.Messages.View;
using Pathfinding.ConsoleApp.ViewModel;
using Terminal.Gui;

namespace Pathfinding.ConsoleApp.View
{
    internal sealed class RandomAlgorithmListItem : Label
    {
        private readonly IMessenger messenger;
        private readonly CreateRandomAlgorithmRunViewModel viewModel;

        public RandomAlgorithmListItem([KeyFilter(KeyFilters.Views)] IMessenger messenger,
            CreateRandomAlgorithmRunViewModel viewModel)
        {
            Text = "Random";
            Y = 1;
            X = 0;
            this.messenger = messenger;
            this.viewModel = viewModel;
            MouseClick += OnViewClicked;
        }

        private void OnViewClicked(MouseEventArgs e)
        {
            if (e.MouseEvent.Flags == MouseFlags.Button1Clicked)
            {
                messenger.Send(new OpenRunCreationViewMessage());
                messenger.Send(new CloseStepRulesViewMessage());
                messenger.Send(new CloseHeuristicsViewMessage());
                messenger.Send(new CloseSpreadViewMessage());
                messenger.Send(new RunViewModelChangedMessage(viewModel));
            }
        }
    }
}
