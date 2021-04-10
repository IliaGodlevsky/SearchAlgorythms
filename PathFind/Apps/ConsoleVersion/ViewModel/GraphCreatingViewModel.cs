﻿using ConsoleVersion.View;
using GraphLib.Exceptions;
using GraphLib.Interface;
using GraphLib.Realizations;
using GraphLib.ViewModel;
using GraphViewModel.Interfaces;

using static ConsoleVersion.InputClass.Input;
using static ConsoleVersion.Constants;

namespace ConsoleVersion.ViewModel
{
    internal class GraphCreatingViewModel : GraphCreatingModel
    {
        public string ObstaclePercentInputMessage { private get; set; }

        public string WidthInputMessage { private get; set; }

        public string HeightInputMessage { private get; set; }

        public GraphCreatingViewModel(IMainModel model,
            IGraphAssembler graphFactory) : base(model, graphFactory)
        {

        }

        public override void CreateGraph()
        {
            ObstaclePercent = InputNumber(ObstaclePercentInputMessage, ObstaclesPercentValueRange);
            Width = InputNumber(WidthInputMessage, GraphWidthValueRange);
            Length = InputNumber(HeightInputMessage, GraphLengthValueRange);

            base.CreateGraph();

            if(!(model.Graph is Graph2D))
            {
                string message = "An error occurred while creating graph\n";
                message += $"Graph must be {nameof(Graph2D)} type";
                throw new WrongGraphTypeException(message, model.Graph);
            }

            MainView.UpdatePositionOfVisualElements(model.Graph);
        }
    }
}
