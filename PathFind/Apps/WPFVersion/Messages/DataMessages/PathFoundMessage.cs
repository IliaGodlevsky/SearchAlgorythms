﻿using Algorithm.Interfaces;

namespace WPFVersion.Messages.DataMessages
{
    internal sealed class PathFoundMessage
    {
        public IAlgorithm Algorithm { get; }
        public IGraphPath Path { get; }

        public PathFoundMessage(IAlgorithm algorithm, IGraphPath path)
        {
            Algorithm = algorithm;
            Path = path;
        }
    }
}