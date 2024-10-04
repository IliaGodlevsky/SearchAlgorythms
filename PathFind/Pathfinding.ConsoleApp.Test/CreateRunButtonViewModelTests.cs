﻿using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Pathfinding.ConsoleApp.Model;
using Pathfinding.ConsoleApp.Model.Factories;
using Pathfinding.ConsoleApp.ViewModel;
using Pathfinding.Domain.Interface;
using Pathfinding.Infrastructure.Business.Layers;
using Pathfinding.Infrastructure.Data.Pathfinding;
using Pathfinding.Infrastructure.Data.Pathfinding.Factories;
using Pathfinding.Service.Interface;
using Pathfinding.Shared.Primitives;
using Pathfinding.Shared.Random;
using Pathfinding.Infrastructure.Data.Extensions;
using Pathfinding.Service.Interface.Models.Read;
using Pathfinding.Shared.Extensions;
using System.Reactive.Linq;
using Pathfinding.ConsoleApp.Messages.ViewModel;
using Pathfinding.Service.Interface.Requests.Create;
using Pathfinding.Logging.Interface;
using Pathfinding.Logging.Loggers;
using Pathfinding.TestUtils.Attributes;
using Pathfinding.Service.Interface.Models.Undefined;
using Pathfinding.Infrastructure.Business.Algorithms;
using Pathfinding.Infrastructure.Business.Algorithms.GraphPaths;
using Moq.Protected;

namespace Pathfinding.ConsoleApp.Test
{
    [TestFixture, UnitTest]
    internal class CreateRunButtonViewModelTests
    {
        private sealed class TestCreateRunButtonViewModel : CreateRunButtonViewModel
        {
            public TestCreateRunButtonViewModel(IRequestService<VertexModel> service,
                IMessenger messenger, ILog logger) : base(service, messenger, logger)
            {
            }

            protected override string AlgorithmId => "Test";

            protected override void AppendStatistics(RunStatisticsModel model)
            {

            }

            protected override PathfindingProcess GetAlgorithm(IEnumerable<VertexModel> pathfindingRange)
            {
                var algorithm = new Mock<PathfindingProcess>();
                algorithm.Setup(x => x.FindPath()).Returns(NullGraphPath.Instance);
                return algorithm.Object;
            }
        }

        private IMessenger messenger;
        private Mock<IRequestService<VertexModel>> service;
        private readonly IGraph<VertexModel> graph;

        public CreateRunButtonViewModelTests()
        {
            var random = new CongruentialRandom();
            var costLayer = new VertexCostLayer((9, 1), range => new VertexCost(random.NextInt(range), range));
            var obstacleLayer = new ObstacleLayer(random, 0);
            var neighborhoodLayer = new NeighborhoodLayer();
            var layers = new Layers(costLayer, obstacleLayer, neighborhoodLayer);
            var vertexFactory = new VertexModelFactory();
            var graphFactory = new GraphFactory<VertexModel>();
            var assemble = new GraphAssemble<VertexModel>(vertexFactory, graphFactory);
            graph = assemble.AssembleGraph(layers, 25, 35);
        }

        [SetUp]
        public void SetUp()
        {
            service = new Mock<IRequestService<VertexModel>>();
            IReadOnlyCollection<PathfindingRangeModel> pathfindingRange 
                = new List<PathfindingRangeModel>()
            {
                new() { Order = 1, Position = new Coordinate(0, 0) },
                new() { Order = 2, Position = new Coordinate(3, 4) },
                new() { Order = 3, Position = new Coordinate(6, 7) },
                new() { Order = 4, Position = new Coordinate(12, 15) },
                new() { Order = 5, Position = new Coordinate(20, 20) }
            };
            service.Setup(x => x.ReadRangeAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(pathfindingRange));
            IReadOnlyCollection<AlgorithmRunHistoryModel> result =
                new AlgorithmRunHistoryModel().Enumerate().ToArray();
            service.Setup(x => x.CreateRunHistoriesAsync(
                    It.IsAny<IEnumerable<CreateAlgorithmRunHistoryRequest>>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(result));
            messenger = new WeakReferenceMessenger();
        }

        [Test]
        public async Task CreateRunCommand_NoActivatedGraph_CantExecute()
        {
            var viewModel = new TestCreateRunButtonViewModel(service.Object,
                messenger, new NullLog());
            var canExecute = await viewModel.CreateRunCommand.CanExecute.FirstOrDefaultAsync();

            Assert.That(canExecute, Is.False);
        }

        [Test]
        public async Task CreateRunCommand_HasActivatedGraph_ShouldExecute()
        {
            var viewModel = new TestCreateRunButtonViewModel(service.Object,
                messenger, new NullLog());
            bool isRunSent = false;
            void OnRunCreated(object recipient, RunCreatedMessaged msg)
            {
                isRunSent = true;
            }
            messenger.Register<RunCreatedMessaged>(this, OnRunCreated);

            messenger.Send(new GraphActivatedMessage(1, graph));
            await viewModel.CreateRunCommand.Execute();

            Assert.Multiple(() =>
            {
                service.Verify(x => x.CreateRunHistoriesAsync(
                    It.IsAny<IEnumerable<CreateAlgorithmRunHistoryRequest>>(),
                    It.IsAny<CancellationToken>()), Times.Once);
                service.Verify(x => x.ReadRangeAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()), Times.Once);
                Assert.That(isRunSent);
            });
        }
    }
}