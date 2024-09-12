﻿using Autofac.Features.AttributeFilters;
using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.ConsoleApp.Injection;
using Pathfinding.ConsoleApp.Messages.View;
using Pathfinding.ConsoleApp.Messages.ViewModel;
using Pathfinding.ConsoleApp.ViewModel;
using Terminal.Gui;

namespace Pathfinding.ConsoleApp.View
{
    internal sealed class IDAStarAlgorithmListItem : Label
    {
        private readonly IMessenger messenger;
        private readonly CreateIDAStarRunViewModel viewModel;

        public IDAStarAlgorithmListItem([KeyFilter(KeyFilters.Views)] IMessenger messenger,
            CreateIDAStarRunViewModel viewModel)
        {
            Text = "IDA*";
            Y = 2;
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
                messenger.Send(new OpenStepRuleViewMessage());
                messenger.Send(new OpenHeuristicsViewMessage());
                messenger.Send(new OpenSpreadViewMessage());
                messenger.Send(new StepRuleViewModelChangedMessage(viewModel));
                messenger.Send(new HeuristicsViewModelChangedMessage(viewModel));
                messenger.Send(new RunViewModelChangedMessage(viewModel));
                messenger.Send(new SpreadViewModelChangedMessage(viewModel));
            }
        }
    }
}
