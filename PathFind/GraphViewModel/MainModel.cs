﻿using Common.Extensions;
using Common.Logging;
using GraphLib.Base;
using GraphLib.Extensions;
using GraphLib.Interface;
using GraphLib.Serialization.Interfaces;
using GraphViewModel.Interfaces;
using System;
using System.IO;

namespace GraphViewModel
{
    public abstract class MainModel : IMainModel
    {
        public BaseEndPoints EndPoints { get; set; }

        public virtual string GraphParametres { get; set; }

        public virtual string PathFindingStatistics { get; set; }

        public virtual IGraphField GraphField { get; set; }

        public virtual IGraph Graph { get; protected set; }

        protected MainModel(BaseGraphFieldFactory fieldFactory,
            IVertexEventHolder eventHolder,
            IGraphSerializer graphSerializer,
            IGraphAssembler graphAssembler,
            IPathInput pathInput)
        {
            this.eventHolder = eventHolder;
            serializer = graphSerializer;
            this.fieldFactory = fieldFactory;
            this.graphAssembler = graphAssembler;
            this.pathInput = pathInput;

            Graph = BaseGraph.NullGraph;
        }

        public virtual async void SaveGraph()
        {
            string savePath = pathInput.InputSavePath();
            try
            {
                using (var stream = new FileStream(savePath, FileMode.OpenOrCreate))
                {
                    await serializer.SaveGraphAsync(Graph, stream);
                }
            }
            catch (Exception ex)
            {
                OnErrorExceptionCaught(ex);
            }
        }

        public virtual void LoadGraph()
        {
            string loadPath = pathInput.InputLoadPath();
            try
            {
                using (var stream = new FileStream(loadPath, FileMode.Open))
                {
                    var newGraph = serializer.LoadGraph(stream);
                    ConnectNewGraph(newGraph);
                }
            }
            catch (Exception ex)
            {
                OnErrorExceptionCaught(ex);
            }
        }

        public abstract void FindPath();

        public abstract void CreateNewGraph();

        public virtual void ClearGraph()
        {
            Graph.Refresh();
            PathFindingStatistics = string.Empty;
            GraphParametres = Graph.ToString();
            EndPoints.Reset();
        }

        public void ConnectNewGraph(IGraph graph)
        {
            if (!graph.IsDefault())
            {
                EndPoints.UnsubscribeFromEvents(Graph);
                EndPoints.Reset();
                eventHolder.UnsubscribeVertices(Graph);
                Graph = graph;
                GraphField = fieldFactory.CreateGraphField(Graph);
                eventHolder.SubscribeVertices(Graph);
                EndPoints.SubscribeToEvents(Graph);
                GraphParametres = Graph.ToString();
                PathFindingStatistics = string.Empty;
            }
        }

        protected abstract string GetAlgorithmsLoadPath();
        protected abstract void OnExceptionCaught(string message);

        protected abstract void OnExceptionCaught(Exception ex, string additaionalMessage = "");

        protected virtual void OnErrorExceptionCaught(Exception ex, string additionalMessage = "")
        {
            OnExceptionCaught(ex, additionalMessage);
            Logger.Instance.Error(ex);
        }

        protected virtual void OnNotFatalExceptionCaught(Exception ex, string additionalMessage = "")
        {
            OnExceptionCaught(ex, additionalMessage);
            Logger.Instance.Warn(ex);
        }


        protected readonly IGraphAssembler graphAssembler;
        protected readonly BaseGraphFieldFactory fieldFactory;
        private readonly IVertexEventHolder eventHolder;
        private readonly IGraphSerializer serializer;
        private readonly IPathInput pathInput;
    }
}
