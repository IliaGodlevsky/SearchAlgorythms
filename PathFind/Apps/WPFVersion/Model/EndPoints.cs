﻿using GraphLib.Base;
using GraphLib.Interfaces;
using System;
using System.Windows.Input;
using System.Windows.Media;

namespace WPFVersion.Model
{
    internal sealed class EndPoints : BaseEndPoints
    {
        protected override void SubscribeVertex(IVertex vertex)
        {
            if (vertex is Vertex vert)
            {
                vert.MouseLeftButtonDown += SetEndPoints;
                vert.MouseUp += MarkIntermediateToReplace;
            }
        }

        protected override void UnsubscribeVertex(IVertex vertex)
        {
            if (vertex is Vertex vert)
            {
                vert.MouseLeftButtonDown -= SetEndPoints;
                vert.MouseUp -= MarkIntermediateToReplace;
            }
        }

        protected override void MarkIntermediateToReplace(object sender, EventArgs e)
        {
            if (e is MouseButtonEventArgs args && args.ChangedButton == MouseButton.Middle)
            {
                if (sender is Vertex vertex)
                {
                    var color = Colors.DarkOrange;
                    color.A -= 70;
                    vertex.Background = new SolidColorBrush(color);
                }
                base.MarkIntermediateToReplace(sender, e);
            }
        }
    }
}
