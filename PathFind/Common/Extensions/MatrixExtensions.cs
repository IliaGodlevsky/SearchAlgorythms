﻿namespace Common.Extensions
{
    public static class MatrixExtensions
    {
        public static int Width<TSource>(this TSource[,] arr)
        {
            return arr == null ? 0 : arr.GetLength(dimension: 0);
        }

        public static int Length<TSource>(this TSource[,] arr)
        {
            if (arr?.Width() == 0)
                return 0;
            return arr.Length / arr.Width();
        }
    }
}