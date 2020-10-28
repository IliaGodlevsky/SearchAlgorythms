﻿using GraphLibrary.Vertex.Interface;
using System.Collections.Generic;

namespace GraphLibrary.Info.Interface
{
    public interface IVertexInfoCollection : IEnumerable<VertexInfo>
    {
        IEnumerable<int> DimensionsSizes { get; }
    }
}
