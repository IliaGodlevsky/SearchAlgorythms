﻿using GraphLib.Interface;
using System.Collections.Generic;

namespace Algorithm.Interfaces
{
    public interface IGraphPath
    {
        IEnumerable<IVertex> Path { get; }
    }
}