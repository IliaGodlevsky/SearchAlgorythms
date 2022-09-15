﻿using Common.Extensions.EnumerableExtensions;
using GraphLib.Interfaces;
using GraphLib.NullRealizations;
using System;
using System.Collections.Generic;
using System.Linq;
using ValueRange;
using ValueRange.Extensions;

namespace GraphLib.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool IsCardinal(this int[] centralCoordinates, int[] cardinalCoordinates)
        {
            // Cardinal coordinate differs from central coordinate only for one coordinate value
            return centralCoordinates.Length == cardinalCoordinates.Length
                ? centralCoordinates.Zip(cardinalCoordinates, (x, y) => x != y).Count(i => i) == 1
                : false;
        }

        public static IVertex FirstOrNullVertex(this IEnumerable<IVertex> collection, Func<IVertex, bool> predicate)
        {
            return collection.FirstOrDefault(predicate) ?? NullVertex.Interface;
        }

        public static int[] ToCoordinates(this int[] dimensionSizes, int index)
        {
            int size = dimensionSizes.AggregateOrDefault((x, y) => x * y);
            var rangeOfIndices = new InclusiveValueRange<int>(size - 1, 0);
            if (!rangeOfIndices.Contains(index))
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var coordinates = new int[dimensionSizes.Length];

            for (int i = 0; i < coordinates.Length; i++)
            {
                coordinates[i] = index % dimensionSizes[i];
                index /= dimensionSizes[i];
            }

            return coordinates;
        }
    }
}
