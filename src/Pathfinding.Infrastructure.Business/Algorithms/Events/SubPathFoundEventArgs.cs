﻿using Pathfinding.Service.Interface;

namespace Pathfinding.Infrastructure.Business.Algorithms.Events
{
    public class SubPathFoundEventArgs : EventArgs
    {
        public IGraphPath SubPath { get; }

        public SubPathFoundEventArgs(IGraphPath subPath)
        {
            SubPath = subPath;
        }
    }
}
