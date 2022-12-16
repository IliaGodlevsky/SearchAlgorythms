﻿using Autofac;
using GalaSoft.MvvmLight.Messaging;
using GraphLib.Serialization.Serializers.Decorators;
using Pathfinding.AlgorithmLib.Core.Abstractions;
using Pathfinding.AlgorithmLib.Core.Interface;
using Pathfinding.AlgorithmLib.Core.Realizations.Heuristics;
using Pathfinding.AlgorithmLib.Core.Realizations.StepRules;
using Pathfinding.AlgorithmLib.Factory;
using Pathfinding.AlgorithmLib.Factory.Interface;
using Pathfinding.App.Console.DependencyInjection.ConfigurationMiddlewears;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.MenuItems;
using Pathfinding.App.Console.MenuItems.GraphMenuItems;
using Pathfinding.App.Console.MenuItems.MainMenuItems;
using Pathfinding.App.Console.MenuItems.PathfindingHistoryMenuItems;
using Pathfinding.App.Console.MenuItems.PathfindingProcessMenuItems;
using Pathfinding.App.Console.MenuItems.PathfindingRangeMenuItems;
using Pathfinding.App.Console.MenuItems.PathfindingStatisticsMenuItems;
using Pathfinding.App.Console.MenuItems.PathfindingVisualizationMenuItems;
using Pathfinding.App.Console.Model;
using Pathfinding.App.Console.Model.InProcessActions;
using Pathfinding.App.Console.Model.PathfindingActions;
using Pathfinding.App.Console.Model.VertexActions;
using Pathfinding.App.Console.ValueInput.UserInput;
using Pathfinding.GraphLib.Core.Interface;
using Pathfinding.GraphLib.Core.Modules;
using Pathfinding.GraphLib.Core.Modules.Commands;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.GraphLib.Factory.Interface;
using Pathfinding.GraphLib.Factory.Realizations;
using Pathfinding.GraphLib.Factory.Realizations.CoordinateFactories;
using Pathfinding.GraphLib.Factory.Realizations.GraphAssembles;
using Pathfinding.GraphLib.Factory.Realizations.GraphFactories;
using Pathfinding.GraphLib.Factory.Realizations.NeighborhoodFactories;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Modules;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Serializers;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Serializers.Decorators;
using Pathfinding.GraphLib.Smoothing.Interface;
using Pathfinding.GraphLib.Smoothing.Realizations.MeanCosts;
using Pathfinding.Logging.Interface;
using Pathfinding.Logging.Loggers;
using Pathfinding.Visualization.Core.Abstractions;
using Pathfinding.VisualizationLib.Core.Interface;
using Shared.Executable;
using Shared.Random;
using Shared.Random.Realizations;
using System;
using System.Globalization;
using static Pathfinding.App.Console.DependencyInjection.RegistrationConstants;

namespace Pathfinding.App.Console.DependencyInjection
{
    using AlgorithmFactory = IAlgorithmFactory<PathfindingProcess>;
    using Command = IPathfindingRangeCommand<Vertex>;
    using GraphSerializer = IGraphSerializer<Graph2D<Vertex>, Vertex>;

    internal static class Registry
    {
        private static readonly Lazy<IContainer> container = new Lazy<IContainer>(Configure);

        public static IContainer Container => container.Value;

        private static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ConsoleUserAnswerInput>().As<IInput<Answer>>().SingleInstance();
            builder.RegisterType<ConsoleUserIntInput>().As<IInput<int>>().SingleInstance();
            builder.RegisterType<ConsoleUserStringInput>().As<IInput<string>>().SingleInstance();
            builder.RegisterType<ConsoleUserKeyInput>().As<IInput<ConsoleKey>>().SingleInstance();
            builder.RegisterType<ConsoleUserTimeSpanInput>().As<IInput<TimeSpan>>().SingleInstance();

            builder.RegisterTypes(Units).SingleInstance().WithMetadata(UnitTypeKey, type => type)
                .AsSelf().AutoActivate().ConfigurePipeline(p => p.Use(new UnitConfigurationMiddlewear()));

            builder.RegisterType<MainUnitMenuItem>().AsSelf().InstancePerDependency();
            builder.RegisterType<GraphCreateMenuItem>().Keyed<IMenuItem>(Main).SingleInstance().AutoActivate();
            builder.RegisterType<PathfindingProcessMenuItem>().Keyed<IMenuItem>(Main).SingleInstance().AutoActivate();
            builder.RegisterType<LanguageMenuItem>().Keyed<IMenuItem>(Main).SingleInstance().AutoActivate();
            builder.RegisterType<PathfindingRangeMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<StatisticsMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<VisualizationMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<HistoryMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();

            builder.Register(_ => new CultureInfo("en-US")).AsSelf().SingleInstance();
            builder.Register(_ => new CultureInfo("ru-RU")).AsSelf().SingleInstance();

            builder.RegisterType<ChangeCostMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<AssembleGraphMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<ResizeGraphMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<EnterCostRangeMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<EnterGraphAssembleMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<EnterGraphParametresMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<EnterObstaclePercentMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<LoadGraphMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<SaveGraphMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<ReverseVertexMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();
            builder.RegisterType<SmoothGraphMenuItem>().Keyed<IMenuItem>(Graph).SingleInstance().AutoActivate();

            builder.RegisterType<ApplyHistoryMenuItem>().Keyed<IMenuItem>(History).SingleInstance().AutoActivate();
            builder.RegisterType<ClearHistoryMenuItem>().Keyed<IMenuItem>(History).SingleInstance().AutoActivate();
            builder.RegisterType<ShowHistoryMenuItem>().Keyed<IMenuItem>(History).SingleInstance().AutoActivate();

            builder.RegisterType<PathfindingAlgorithmMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<ClearColorsMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<ClearGraphMenuItem>().Keyed<IMenuItem>(Process).SingleInstance().AutoActivate();
            builder.RegisterType<ClearPathfindingRangeMenuItem>().Keyed<IMenuItem>(Range).SingleInstance().AutoActivate();
            builder.RegisterType<EnterPathfindingRangeMenuItem>().Keyed<IMenuItem>(Range).SingleInstance().AutoActivate()
                .ConfigurePipeline(p => p.Use(new EnterRangeConfigurationMiddlewear()));
            builder.RegisterType<MarkTransitToReplaceAction>().As<IVertexAction>().SingleInstance().WithMetadata(Key, ConsoleKey.R);
            builder.RegisterType<ReplaceTransitVertexAction>().As<IVertexAction>().SingleInstance().WithMetadata(Key, ConsoleKey.P);

            builder.RegisterType<ApplyStatisticsMenuItem>().Keyed<IMenuItem>(Statistics).SingleInstance().AutoActivate();

            builder.RegisterType<ApplyVisualizationMenuItem>().Keyed<IMenuItem>(Visual).SingleInstance().AutoActivate();
            builder.RegisterType<EnterAnimationDelayMenuItem>().Keyed<IMenuItem>(Visual).SingleInstance().AutoActivate();

            builder.RegisterType<ViewFactory>().As<IViewFactory>().SingleInstance();

            builder.RegisterType<SpeedUpAnimation>().As<IAnimationSpeedAction>().WithMetadata(Key, ConsoleKey.UpArrow).SingleInstance();
            builder.RegisterType<SlowDownAnimation>().As<IAnimationSpeedAction>().WithMetadata(Key, ConsoleKey.DownArrow).SingleInstance();

            builder.RegisterType<ResumeAlgorithm>().As<IPathfindingAction>().WithMetadata(Key, ConsoleKey.Enter).SingleInstance();
            builder.RegisterType<PauseAlgorithm>().As<IPathfindingAction>().WithMetadata(Key, ConsoleKey.P).SingleInstance();
            builder.RegisterType<InterruptAlgorithm>().As<IPathfindingAction>().WithMetadata(Key, ConsoleKey.Escape).SingleInstance();
            builder.RegisterType<PathfindingStepByStep>().As<IPathfindingAction>().WithMetadata(Key, ConsoleKey.W).SingleInstance();

            builder.RegisterType<FileLog>().As<ILog>().SingleInstance();
            builder.RegisterType<ColorConsoleLog>().As<ILog>().SingleInstance();
            builder.RegisterType<MailLog>().As<ILog>().SingleInstance();
            builder.RegisterComposite<Logs, ILog>().SingleInstance();

            builder.RegisterComposite<CompositeUndo, IUndo>().SingleInstance();
            builder.RegisterType<Messenger>().As<IMessenger>().SingleInstance();
            builder.RegisterType<PseudoRandom>().As<IRandom>().SingleInstance();
            builder.RegisterType<RootMeanSquareCost>().As<IMeanCost>().SingleInstance();

            builder.RegisterType<VisualPathfindingRange<Vertex>>().As<IPathfindingRange<Vertex>>().SingleInstance();
            builder.RegisterType<PathfindingRangeBuilder<Vertex>>().As<IPathfindingRangeBuilder<Vertex>>().As<IUndo>()
                .SingleInstance().ConfigurePipeline(p => p.Use(new RangeBuilderConfigurationMiddlewear()));
            builder.RegisterType<IncludeSourceVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 2).SingleInstance();
            builder.RegisterType<IncludeTargetVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 4).SingleInstance();
            builder.RegisterType<IncludeTransitVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 6).SingleInstance();
            builder.RegisterType<ReplaceTransitIsolatedVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 1).SingleInstance();
            builder.RegisterType<ReplaceIsolatedSourceVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 3).SingleInstance();
            builder.RegisterType<ReplaceIsolatedTargetVertex<Vertex>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 5).SingleInstance();
            builder.RegisterType<ExcludeTransitVertex<Vertex>>().Keyed<Command>(ExcludeCommand).WithMetadata(Order, 3).SingleInstance();
            builder.RegisterType<ExcludeSourceVertex<Vertex>>().Keyed<Command>(ExcludeCommand).WithMetadata(Order, 1).SingleInstance();
            builder.RegisterType<ExcludeTargetVertex<Vertex>>().Keyed<Command>(ExcludeCommand).WithMetadata(Order, 2).SingleInstance();
            builder.RegisterType<ReplaceTransitVerticesModule<Vertex>>().AsSelf().As<IUndo>().SingleInstance();

            builder.RegisterType<GraphAssemble<Graph2D<Vertex>, Vertex>>().As<IGraphAssemble<Graph2D<Vertex>, Vertex>>().SingleInstance();
            builder.RegisterType<CostFactory>().As<IVertexCostFactory>().SingleInstance();
            builder.RegisterType<VertexFactory>().As<IVertexFactory<Vertex>>().SingleInstance();
            builder.RegisterType<GraphFieldFactory>().As<IGraphFieldFactory<Graph2D<Vertex>, Vertex, GraphField>>().SingleInstance();
            builder.RegisterType<Coordinate2DFactory>().As<ICoordinateFactory>().SingleInstance();
            builder.RegisterType<Graph2DFactory<Vertex>>().As<IGraphFactory<Graph2D<Vertex>, Vertex>>().SingleInstance();
            builder.RegisterDecorator<Graph2dWrapFactory, IGraphFactory<Graph2D<Vertex>, Vertex>>();
            builder.RegisterType<MooreNeighborhoodFactory>().As<INeighborhoodFactory>().SingleInstance();
            builder.RegisterType<VertexVisualization>().As<IVisualization<Vertex>>().SingleInstance();

            builder.RegisterType<InFileSerializationModule<Graph2D<Vertex>, Vertex>>()
                .As<IGraphSerializationModule<Graph2D<Vertex>, Vertex>>().SingleInstance();
            builder.RegisterType<PathInput>().As<IPathInput>().SingleInstance().PropertiesAutowired();
            builder.RegisterType<BinaryGraphSerializer<Graph2D<Vertex>, Vertex>>().As<GraphSerializer>().SingleInstance();
            builder.RegisterDecorator<CompressGraphSerializer<Graph2D<Vertex>, Vertex>, GraphSerializer>();
            builder.RegisterDecorator<CryptoGraphSerializer<Graph2D<Vertex>, Vertex>, GraphSerializer>();
            builder.RegisterType<VertexFromInfoFactory>().As<IVertexFromInfoFactory<Vertex>>().SingleInstance();

            builder.RegisterType<DijkstraAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<AStarAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<IDAStarAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<LeeAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<AStarLeeAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<CostGreedyAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<DistanceFirstAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<DepthFirstAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<HeuristicCostGreedyAlgorithmFactory>().As<AlgorithmFactory>().SingleInstance();
            builder.RegisterType<LandscapeStepRule>().As<IStepRule>().SingleInstance();
            builder.RegisterType<ManhattanDistance>().As<IHeuristic>().SingleInstance();

            return builder.Build();
        }
    }
}