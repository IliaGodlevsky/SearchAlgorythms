﻿using Pathfinding.GraphLib.Core.Interface;
using Pathfinding.GraphLib.Core.Interface.Extensions;
using Pathfinding.GraphLib.Serialization.Core.Interface;
using Pathfinding.GraphLib.Serialization.Core.Realizations.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pathfinding.GraphLib.UnitTest.Realizations.TestObjects
{
    [DebuggerDisplay("{Position.ToString()}")]
    public sealed class TestVertex : IVertex
    {
        public TestVertex(ICoordinate coordinate)
        {
            this.InitializeComponents();
            Position = coordinate;
        }

        public TestVertex(VertexSerializationInfo info)
            : this(info.Position)
        {
            this.Initialize(info);
        }

        public bool IsObstacle { get; set; }

        public IVertexCost Cost { get; set; }

        public IReadOnlyCollection<IVertex> Neighbours { get; set; }

        public ICoordinate Position { get; }

        public bool Equals(IVertex other)
        {
            return other.IsEqual(this);
        }

        public override bool Equals(object obj)
        {
            return obj is IVertex vertex && Equals(vertex);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Cost, Position);
        }
    }
}