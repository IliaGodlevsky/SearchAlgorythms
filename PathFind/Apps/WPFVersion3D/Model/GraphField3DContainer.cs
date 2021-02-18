﻿using GraphLib.Interface;
using System.Windows;
using System.Windows.Media.Media3D;

namespace WPFVersion3D.Model
{
    internal class GraphField3DContainer : ModelVisual3D
    {
        public static readonly DependencyProperty GraphFieldProperty;

        static GraphField3DContainer()
        {
            GraphFieldProperty = DependencyProperty.Register(
                nameof(GraphField),
                typeof(IGraphField),
                typeof(GraphField3DContainer),
                new PropertyMetadata(OnGraphFieldChanged));
        }

        public IGraphField GraphField
        {
            get => (GraphField3D)GetValue(GraphFieldProperty);
            set => SetValue(GraphFieldProperty, value);
        }

        protected static void OnGraphFieldChanged(DependencyObject depObj, 
            DependencyPropertyChangedEventArgs args)
        {
            var field = depObj as GraphField3DContainer;
            if (args.NewValue is GraphField3D child)
            {
                field.Children.Clear();
                field.Children.Add(child);
            }
        }
    }
}
