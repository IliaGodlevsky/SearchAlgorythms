﻿using Shared.Extensions;
using Shared.Primitives.ValueRange;
using System;
using System.Collections.Generic;

namespace Shared.Primitives.Extensions
{
    public static class InclusiveValueRangeExtensions
    {
        public struct RangeEnumerator
        {
            private readonly int start;
            private readonly int end;

            public int Current { get; private set; }

            internal RangeEnumerator(int start, int end)
            {
                this.start = start;
                this.end = end;
                Current = start - 1;
            }

            internal RangeEnumerator(InclusiveValueRange<int> range)
                : this(range.LowerValueOfRange, range.UpperValueOfRange)
            {

            }

            public bool MoveNext()
            {
                return ++Current <= end;
            }

            public void Reset()
            {
                Current = start - 1;
            }
        }

        /// <summary>
        /// Returns all values from <paramref name="range"/>
        /// </summary>
        /// <param name="range"></param>
        /// <returns>An enumerable, that contains all 
        /// values from <paramref name="range"/></returns>
        public static IEnumerable<int> Iterate(this InclusiveValueRange<int> range)
        {
            foreach (int value in range)
            {
                yield return value;
            }
        }

        public static RangeEnumerator GetEnumerator(this InclusiveValueRange<int> range)
        {
            return new RangeEnumerator(range);
        }

        public static long Amplitude(this InclusiveValueRange<int> valueRange)
        {
            return (long)valueRange.UpperValueOfRange - valueRange.LowerValueOfRange;
        }

        public static double Amplitude(this InclusiveValueRange<double> valueRange)
        {
            return valueRange.UpperValueOfRange - valueRange.LowerValueOfRange;
        }

        public static bool Contains<T>(this InclusiveValueRange<T> self, T value)
            where T : IComparable<T>
        {
            return value.IsBetween(self.UpperValueOfRange, self.LowerValueOfRange);
        }

        public static T ReturnInRange<T>(this InclusiveValueRange<T> self, T value, ReturnOptions options)
            where T : IComparable<T>
        {
            return options.ReturnInRange(value, self);
        }

        public static T ReturnInRange<T>(this InclusiveValueRange<T> self, T value)
            where T : IComparable<T>
        {
            return self.ReturnInRange(value, ReturnOptions.Limit);
        }
    }
}
