﻿using GraphLib.EventHolder.Interface;
using GraphLib.GraphField;
using GraphLib.Graphs.Abstractions;

namespace GraphViewModel.Interfaces
{
    public interface IMainModel : IModel
    {
        IVertexEventHolder VertexEventHolder { get; set; }
        string GraphParametres { get; set; }
        IGraphField GraphField { get; set; }
        string PathFindingStatistics { get; set; }
        void ConnectNewGraph(IGraph graph);
        void CreateNewGraph();
        IGraph Graph { get; }
        void SaveGraph();
        void ClearGraph();
        void LoadGraph();
        void FindPath();
    }
}