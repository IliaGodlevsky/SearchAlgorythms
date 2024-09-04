﻿using Pathfinding.ConsoleApp.ViewModel.ButtonsFrameViewModels;
using Terminal.Gui;
using ReactiveMarbles.ObservableEvents;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Pathfinding.ConsoleApp.View.ButtonsFrameViews
{
    internal sealed partial class LoadGraphButton : Button
    {
        private readonly LoadGraphButtonModel viewModel;

        public LoadGraphButton(LoadGraphButtonModel viewModel)
        {
            this.viewModel = viewModel;
            Initialize();
            this.Events()
                .MouseClick
                .Where(x => x.MouseEvent.Flags == MouseFlags.Button1Clicked)
                .Do(async x=> await OnLoadButtonClicked(x))
                .InvokeCommand(viewModel, x=>x.LoadGraphCommand);
        }

        private async Task OnLoadButtonClicked(MouseEventArgs e)
        {
            var allowedTypes = new List<string>() { ".json" };
            using (var dialog = new OpenDialog("Import", string.Empty, allowedTypes))
            {
                dialog.Width = Dim.Percent(45);
                dialog.Height = Dim.Percent(55);
                Application.Run(dialog);
                if (!dialog.Canceled && dialog.FilePath != null)
                {
                    viewModel.FilePath = dialog.FilePath.ToString();
                }
            }
            await Task.CompletedTask;
        }
    }
}
