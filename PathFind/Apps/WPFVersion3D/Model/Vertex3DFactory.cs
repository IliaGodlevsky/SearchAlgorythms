﻿using GraphLib.Interfaces;
using GraphLib.Interfaces.Factories;
using System.Windows;
using System.Windows.Threading;
using WPFVersion3D.Interface;

namespace WPFVersion3D.Model
{
    internal sealed class Vertex3DFactory : IVertexFactory<Vertex3D>
    {
        private static readonly Dispatcher Dispatcher = Application.Current.Dispatcher;

        private readonly IModel3DFactory model3Dfactory;
        private readonly IVisualization<Vertex3D> visualization;

        public Vertex3DFactory(IModel3DFactory modelFactory, IVisualization<Vertex3D> visualization)
        {
            model3Dfactory = modelFactory;
            this.visualization = visualization;
        }

        public Vertex3D CreateVertex(ICoordinate coordinate)
        {
            return Dispatcher.Invoke(() => new Vertex3D(coordinate, model3Dfactory, visualization));
        }
    }
}
