﻿using Conditional;
using GraphLib.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphLib.Base
{
    public abstract class BaseVertexEventHolder : IVertexEventHolder
    {
        public BaseVertexEventHolder()
        {
            reverseActionDictionary = new Dictionary<bool, Action<IVertex>>()
            {
                { true, vertex => (vertex as IMarkable)?.MarkAsObstacle() },
                { false, vertex => (vertex as IMarkable)?.MarkAsRegular() }
            };

            If = new If(o => (o as IVertex).IsObstacle, o => MakeVertex((IVertex)o))
                   .Else(o => MakeObstacle((IVertex)o));
        }

        public IVertexCostFactory CostFactory { get; set; }

        public IGraph Graph { get; set; }

        public virtual void ChangeVertexCost(object sender, EventArgs e)
        {
            if (sender is IVertex vertex && !vertex.IsObstacle)
            {
                int delta = GetWheelDelta(e) > 0 ? 1 : -1;
                int newCost = vertex.Cost.CurrentCost + delta;
                vertex.Cost = CostFactory.CreateCost(newCost);
            }
        }

        public virtual void Reverse(object sender, EventArgs e)
        {
            If.Walk(sender as IVertex, 
                param => param is IVertex);            
        }

        public void UnsubscribeVertices(IGraph graph)
        {
            graph.Vertices.AsParallel().ForAll(UnsubscribeFromEvents);
        }

        public void SubscribeVertices(IGraph graph)
        {
            graph.Vertices.AsParallel().ForAll(SubscribeToEvents);
        }

        protected abstract void UnsubscribeFromEvents(IVertex vertex);

        protected abstract void SubscribeToEvents(IVertex vertex);

        protected abstract int GetWheelDelta(EventArgs e);

        private void MakeObstacle(IVertex vertex)
        {
            reverseActionDictionary[vertex.IsObstacle = true](vertex);
        }

        private void MakeVertex(IVertex vertex)
        {
            reverseActionDictionary[vertex.IsObstacle = false](vertex);
        }

        private readonly Dictionary<bool, Action<IVertex>> reverseActionDictionary;

        private If If { get; }
    }
}
