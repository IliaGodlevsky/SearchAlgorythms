﻿using GalaSoft.MvvmLight.Messaging;
using GraphLib.Serialization.Serializers.Decorators;
using Pathfinding.Logging.Interface;
using System;
using System.Reflection;
using Pathfinding.Logging.Loggers;
using Pathfinding.App.WPF._3D.Model;
using Pathfinding.App.WPF._3D.ViewModel;
using Pathfinding.GraphLib.Subscriptions;
using Pathfinding.GraphLib.Core.Interface;
using Autofac;
using Pathfinding.App.WPF._3D.Extensions;
using Shared.Extensions;
using Pathfinding.App.WPF._3D.Interface;
using Shared.Random.Realizations;
using Shared.Random;
using Pathfinding.GraphLib.Factory.Interface;
using Pathfinding.GraphLib.Factory.Realizations.CoordinateFactories;
using Pathfinding.GraphLib.Core.Realizations.Graphs;
using Pathfinding.GraphLib.Factory.Realizations.GraphFactories;
using Pathfinding.GraphLib.Factory.Realizations.NeighborhoodFactories;
using Pathfinding.VisualizationLib.Core.Interface;
using Pathfinding.App.WPF._3D.Model3DFactories;
using Pathfinding.GraphLib.Factory.Realizations.GraphAssembles;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Modules;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Serializers;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Serializers.Decorators;
using Pathfinding.AlgorithmLib.Factory.Interface;
using Pathfinding.AlgorithmLib.Core.Abstractions;
using Shared.Executable;
using Pathfinding.Visualization.Core.Abstractions;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Pathfinding.GraphLib.Core.Modules.Interface;
using Pathfinding.GraphLib.Core.Modules;
using Pathfinding.GraphLib.Core.Modules.Commands;
using Pathfinding.GraphLib.Factory.Realizations;

using static Pathfinding.App.WPF._3D.DependencyInjection.RegistrationConstants;

namespace Pathfinding.App.WPF._3D.DependencyInjection
{
    using Command = IPathfindingRangeCommand<Vertex3D>;
    using AlgorithmFactory = IAlgorithmFactory<PathfindingProcess>;

    internal static class DI
    {
        private static readonly Lazy<IContainer> container = new Lazy<IContainer>(Configure);

        public static IContainer Container => container.Value;

        private static Assembly[] Assemblies => AppDomain.CurrentDomain.GetAssemblies();

        private static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainWindowViewModel>().AsSelf().AsImplementedInterfaces().SingleInstance();
            builder.RegisterAssemblyTypes(Assemblies).Where(type => type.Implements<IViewModel>()).AsSelf().InstancePerDependency().PropertiesAutowired();
            builder.RegisterAssemblyTypes(Assemblies).Where(type => type.IsAppWindow()).AsSelf().InstancePerDependency();

            builder.RegisterType<Messenger>().As<IMessenger>().SingleInstance();

            builder.RegisterType<VisualPathfindingRange<Vertex3D>>().As<IPathfindingRange<Vertex3D>>().SingleInstance();
            builder.RegisterType<PathfindingRangeBuilder<Vertex3D>>().As<IPathfindingRangeBuilder<Vertex3D>>().As<IUndo>()
                .SingleInstance().ConfigurePipeline(p => p.Use(new RangeBuilderConfigurationMiddlewear()));
            builder.RegisterType<IncludeSourceVertex<Vertex3D>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 1).SingleInstance();
            builder.RegisterType<IncludeTargetVertex<Vertex3D>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 3).SingleInstance();            
            builder.RegisterType<ReplaceIsolatedSourceVertex<Vertex3D>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 2).SingleInstance();
            builder.RegisterType<ReplaceIsolatedTargetVertex<Vertex3D>>().Keyed<Command>(IncludeCommand).WithMetadata(Order, 4).SingleInstance();
            builder.RegisterType<ExcludeSourceVertex<Vertex3D>>().Keyed<Command>(ExcludeCommand).WithMetadata(Order, 1).SingleInstance();
            builder.RegisterType<ExcludeTargetVertex<Vertex3D>>().Keyed<Command>(ExcludeCommand).WithMetadata(Order, 2).SingleInstance();           

            builder.RegisterType<FileLog>().As<ILog>().SingleInstance();
            builder.RegisterType<MessageBoxLog>().As<ILog>().SingleInstance();
            builder.RegisterType<MailLog>().As<ILog>().SingleInstance();
            builder.RegisterComposite<Logs, ILog>().SingleInstance();

            builder.RegisterType<VertexReverseModuleSubscription>().As<IGraphSubscription<Vertex3D>>().SingleInstance();
            builder.RegisterType<PathfindingRangeBuilderSubscription>().As<IGraphSubscription<Vertex3D>>().SingleInstance();
            builder.RegisterComposite<GraphSubscriptions<Vertex3D>, IGraphSubscription<Vertex3D>>().SingleInstance();

            builder.RegisterComposite<CompositeUndo, IUndo>().SingleInstance();

            builder.RegisterType<KnuthRandom>().As<IRandom>().SingleInstance();
            builder.RegisterDecorator<ThreadSafeRandom, IRandom>();

            builder.RegisterType<Vertex3DFactory>().As<IVertexFactory<Vertex3D>>().SingleInstance();
            builder.RegisterType<CostFactory>().As<IVertexCostFactory>().SingleInstance();
            builder.RegisterType<Coordinate3DFactory>().As<ICoordinateFactory>().SingleInstance();
            builder.RegisterType<Graph3DFactory<Vertex3D>>().As<IGraphFactory<Graph3D<Vertex3D>, Vertex3D>>().SingleInstance();
            builder.RegisterType<GraphField3DFactory>().As<IGraphFieldFactory<Graph3D<Vertex3D>, Vertex3D, GraphField3D>>().SingleInstance();
            builder.RegisterType<VonNeumannNeighborhoodFactory>().As<INeighborhoodFactory>().SingleInstance();
            builder.RegisterType<CubicModel3DFactory>().As<IModel3DFactory>().SingleInstance();
            builder.RegisterType<GraphAssemble<Graph3D<Vertex3D>, Vertex3D>>().As<IGraphAssemble<Graph3D<Vertex3D>, Vertex3D>>().SingleInstance();           
            builder.RegisterType<VertexVisualization>().As<IVisualization<Vertex3D>>().SingleInstance();

            builder.RegisterType<InFileSerializationModule<Graph3D<Vertex3D>, Vertex3D>>().As<IGraphSerializationModule<Graph3D<Vertex3D>, Vertex3D>>().SingleInstance();
            builder.RegisterType<PathInput>().As<IPathInput>().SingleInstance();
            builder.RegisterType<XmlGraphSerializer<Graph3D<Vertex3D>, Vertex3D>>().As<IGraphSerializer<Graph3D<Vertex3D>, Vertex3D>>().SingleInstance();
            builder.RegisterDecorator<CompressGraphSerializer<Graph3D<Vertex3D>, Vertex3D>, IGraphSerializer<Graph3D<Vertex3D>, Vertex3D>>();
            builder.RegisterDecorator<CryptoGraphSerializer<Graph3D<Vertex3D>, Vertex3D>, IGraphSerializer<Graph3D<Vertex3D>, Vertex3D>>();
            builder.RegisterType<Vertex3DFromInfoFactory>().As<IVertexFromInfoFactory<Vertex3D>>().SingleInstance();

            builder.RegisterAssemblyTypes(Assemblies).Where(type => type.Implements<AlgorithmFactory>()).As<AlgorithmFactory>().SingleInstance();

            return builder.Build();
        }
    }
}