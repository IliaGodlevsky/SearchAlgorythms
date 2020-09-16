﻿using GraphLibrary.DTO;
using GraphLibrary.Vertex.Interface;
using System.Collections.Generic;

namespace GraphLibrary.Vertex
{
    public sealed class NullVertex : IVertex
    {
        private static NullVertex instance = null;

        public static NullVertex Instance
        {
            get
            {
                if (instance == null)
                    instance = new NullVertex();
                return instance;
            }
        }

        private NullVertex()
        {
            isEnd = true;
            isObstacle = false;
            isStart = true;
            isVisited = false;
            cost = 0;
            neighbours = new List<IVertex>();
            accumulatedCost = double.PositiveInfinity;
            position = new Position(0, 0);
        }

        private bool isEnd;
        public bool IsEnd { get => isEnd; set => isEnd = true; }

        private bool isObstacle;
        public bool IsObstacle { get => isObstacle; set => isObstacle = false; }

        private bool isStart;
        public bool IsStart { get => isStart; set => isStart = true; }

        private bool isVisited;
        public bool IsVisited { get => isVisited; set => isVisited = false; }

        private int cost;
        public int Cost { get => cost; set => cost = 0; }

        private List<IVertex> neighbours;
        public List<IVertex> Neighbours { get => neighbours; set => neighbours = new List<IVertex>(); }

        private IVertex parentVertex;
        public IVertex ParentVertex { get => Instance; set => parentVertex = Instance; }

        private double accumulatedCost;
        public double AccumulatedCost { get => accumulatedCost; set => accumulatedCost = double.PositiveInfinity; }

        private Position position;
        public Position Position { get => position; set => position = new Position(0, 0); }

        public VertexDto Dto => new VertexDto(this);

        public void MarkAsEnd() { }
        public void MarkAsObstacle() { }
        public void MarkAsPath() { }
        public void MarkAsSimpleVertex() { }
        public void MarkAsStart() { }
        public void MarkAsVisited() { }
    }
}
