﻿using CommunityToolkit.Mvvm.Messaging;
using Moq;
using Pathfinding.ConsoleApp.Model;
using Pathfinding.ConsoleApp.Model.Factories;
using Pathfinding.Domain.Interface;
using Pathfinding.Infrastructure.Business.Layers;
using Pathfinding.Infrastructure.Data.Pathfinding;
using Pathfinding.Infrastructure.Data.Pathfinding.Factories;
using Pathfinding.Service.Interface;
using Pathfinding.Shared.Random;
using Pathfinding.TestUtils.Attributes;
using Pathfinding.Infrastructure.Data.Extensions;
using Pathfinding.Shared.Extensions;
using Pathfinding.ConsoleApp.ViewModel;
using Pathfinding.Logging.Loggers;
using Pathfinding.ConsoleApp.Messages.ViewModel;
using System.Reactive.Linq;
using Pathfinding.Service.Interface.Requests.Update;
using System.Collections;

namespace Pathfinding.ConsoleApp.Test
{
    [TestFixture, UnitTest]
    internal class GraphFieldViewModelTests
    {
        private IMessenger messenger;
        private Mock<IRequestService<VertexModel>> service;
        private readonly IGraph<VertexModel> graph;

        [SetUp]
        public void SetUp()
        {
            service = new Mock<IRequestService<VertexModel>>();
            messenger = new WeakReferenceMessenger();
        }

        public GraphFieldViewModelTests()
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

        [TestCaseSource(nameof(ReverseTestData))]
        public async Task ReverseVertexCommand_VertexIsNotInRange_ShouldReverse(bool isInRange,
            bool shouldBeChanged, Times serviceCallsCount)
        {
            var viewModel = new GraphFieldViewModel(messenger, service.Object, new NullLog());
            messenger.Send(new GraphActivatedMessage(1, graph));
            void IsInRangeRecieved(object recipient, IsVertexInRangeRequest msg)
            {
                msg.IsInRange = isInRange;
            }
            bool isObstaclesChanged = false;
            void ObstaclesChanged(object recipient, ObstaclesCountChangedMessage msg)
            {
                isObstaclesChanged = true;
            }

            messenger.Register<IsVertexInRangeRequest>(this, IsInRangeRecieved);
            messenger.Register<ObstaclesCountChangedMessage>(this, ObstaclesChanged);

            var vertex = graph.Get(7, 23);
            await viewModel.ReverseVertexCommand.Execute(vertex);

            Assert.Multiple(() =>
            {
                service.Verify(x => x.UpdateVerticesAsync(
                    It.IsAny<UpdateVerticesRequest<VertexModel>>(),
                    It.IsAny<CancellationToken>()), serviceCallsCount);
                service.Verify(x => x.UpdateObstaclesCountAsync(
                    It.IsAny<UpdateGraphInfoRequest>(),
                    It.IsAny<CancellationToken>()), serviceCallsCount);
                Assert.That(isObstaclesChanged == shouldBeChanged);
                Assert.That(vertex.IsObstacle == shouldBeChanged);
            });
        }

        private static IEnumerable ReverseTestData
        {
            get
            {
                yield return new TestCaseData(true, false, Times.Never());
                yield return new TestCaseData(false, true, Times.Once());
            }
        }
    }
}