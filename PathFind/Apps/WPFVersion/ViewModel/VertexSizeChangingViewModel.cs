﻿using Common.Extensions;
using Common.Interface;
using GraphLib.Base;
using GraphLib.Interface;
using GraphViewModel.Interfaces;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFVersion.Infrastructure;
using WPFVersion.Model;

namespace WPFVersion.ViewModel
{
    internal class VertexSizeChangingViewModel : IModel, IViewModel
    {
        public event EventHandler OnWindowClosed;

        public int VerticesSize { get; set; }

        public MainWindowViewModel Model { get; set; }

        public ICommand ChangeVertexSizeCommand { get; }
        public ICommand CancelCommand { get; }

        public VertexSizeChangingViewModel(MainWindowViewModel model, BaseGraphFieldFactory fieldFactory)
        {
            Model = model;
            this.fieldFactory = fieldFactory;

            ChangeVertexSizeCommand = new RelayCommand(ExecuteChangeVerticesSizeCommand);
            CancelCommand = new RelayCommand(ExecuteCancelCommand);

            if (Model.Graph.Vertices.Any())
            {
                VerticesSize = GetSampleSizeToChange();
            }
        }

        private int GetSampleSizeToChange()
        {
            var randomVertex = Model.Graph.Vertices.GetRandomElement();
            if (randomVertex is Vertex vertex)
            {
                return Convert.ToInt32(vertex.Width);
            }
            return Constants.VertexSize;
        }

        private void ChangeSize(IVertex vertex)
        {
            if (vertex is Vertex temp)
            {
                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    temp.Width = VerticesSize;
                    temp.Height = VerticesSize;
                    temp.FontSize = VerticesSize * Constants.TextToSizeRatio;
                });
            }
        }

        private void CreateNewGraphField()
        {
            Model.GraphField.Clear();
            Model.GraphField = fieldFactory.CreateGraphField(Model.Graph);
        }

        private void ExecuteChangeVerticesSizeCommand(object param)
        {
            Model.Graph.Vertices.ForEach(ChangeSize);
            CreateNewGraphField();
            OnWindowClosed?.Invoke(this, EventArgs.Empty);
        }

        private void ExecuteCancelCommand(object param)
        {
            OnWindowClosed?.Invoke(this, EventArgs.Empty);
        }

        private readonly BaseGraphFieldFactory fieldFactory;
    }
}
