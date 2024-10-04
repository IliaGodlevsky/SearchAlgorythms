﻿using Autofac.Features.AttributeFilters;
using CommunityToolkit.Mvvm.Messaging;
using Pathfinding.ConsoleApp.Injection;
using Pathfinding.ConsoleApp.Messages.ViewModel;
using Pathfinding.ConsoleApp.Model;
using Pathfinding.Logging.Interface;
using Pathfinding.Service.Interface;
using Pathfinding.Service.Interface.Models.Serialization;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using static Terminal.Gui.View;

namespace Pathfinding.ConsoleApp.ViewModel
{
    internal sealed class LoadGraphButtonViewModel : BaseViewModel
    {
        private readonly IMessenger messenger;
        private readonly IRequestService<VertexModel> service;
        private readonly Func<string, Stream> fileStreamProvider;
        private readonly ILog logger;
        private readonly ISerializer<List<PathfindingHistorySerializationModel>> serializer;

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set => this.RaiseAndSetIfChanged(ref filePath, value);
        }

        public ReactiveCommand<MouseEventArgs, Unit> LoadGraphCommand { get; }

        public LoadGraphButtonViewModel([KeyFilter(KeyFilters.ViewModels)] IMessenger messenger,
            ISerializer<List<PathfindingHistorySerializationModel>> serializer,
            IRequestService<VertexModel> service,
            ILog logger) : this(messenger, serializer, 
                service, path => File.OpenRead(path), logger)
        {
        }

        public LoadGraphButtonViewModel(IMessenger messenger,
            ISerializer<List<PathfindingHistorySerializationModel>> serializer,
            IRequestService<VertexModel> service,
            Func<string, Stream> fileStreamProvider,
            ILog logger)
        {
            this.messenger = messenger;
            this.serializer = serializer;
            this.service = service;
            this.logger = logger;
            this.fileStreamProvider = fileStreamProvider;
            LoadGraphCommand = ReactiveCommand.CreateFromTask<MouseEventArgs>(LoadGraph);
        }

        private async Task LoadGraph(MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                await ExecuteSafe(async () =>
                {
                    using var fileStream = fileStreamProvider(FilePath);
                    var histories = await serializer.DeserializeFromAsync(fileStream)
                        .ConfigureAwait(false);
                    var result = await service.CreatePathfindingHistoriesAsync(histories)
                        .ConfigureAwait(false);
                    var graphs = result.Select(x => x.Graph).ToArray();
                    messenger.Send(new GraphCreatedMessage(graphs));
                }, logger.Error);
            }
        }
    }
}