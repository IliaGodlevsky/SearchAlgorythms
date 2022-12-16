﻿using NUnit.Framework;
using Pathfinding.AlgorithmLib.Core.Interface;
using Pathfinding.AlgorithmLib.Factory.Interface;
using Pathfinding.GraphLib.Core.Interface;
using System.Collections.Generic;
using System.Linq;

namespace Pathfinding.AlgorithmLib.Core.Realizations.Tests
{
    using AlgorithmFactory = IAlgorithmFactory<IAlgorithm<IGraphPath>>;
    using Range = IEnumerable<IVertex>;

    [TestFixture]
    public class PathfindingAlgorithmTests
    {
        [TestCaseSource(typeof(FindPathTestCaseData), nameof(FindPathTestCaseData.Data))]
        public void FindPath_ReturnsOptimalPath(AlgorithmFactory factory, Range range, IGraphPath expected)
        {
            var algorithm = factory.Create(range);

            var path = algorithm.FindPath();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(path.Cost, expected.Cost, "The path cost is not equal to expected");
                Assert.AreEqual(path.Count, expected.Count, "The path length is not equal to expected");
                Assert.IsTrue(path.SequenceEqual(expected), "Path contains wrong sequence of coordinates");
            });
        }
    }
}