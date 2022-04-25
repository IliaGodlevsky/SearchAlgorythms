﻿using GraphLib.Base;
using System;

namespace GraphLib.TestRealizations.TestObjects
{
    [Serializable]
    public sealed class TestVertexCost : BaseVertexCost
    {
        public TestVertexCost(int cost)
            : base(cost)
        {

        }

        public override string ToString()
        {
            return CurrentCost.ToString();
        }
    }
}
