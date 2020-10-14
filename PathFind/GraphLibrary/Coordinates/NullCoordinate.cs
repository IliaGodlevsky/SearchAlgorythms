﻿using GraphLibrary.Coordinates.Interface;
using System.Collections.Generic;

namespace GraphLibrary.Coordinates
{
    public sealed class NullCoordinate : ICoordinate
    {
        public static NullCoordinate Instance
        {
            get
            {
                if (instance == null)
                    instance = new NullCoordinate();
                return instance;
            }
        }

        public IEnumerable<int> Coordinates => new int[] { };

        public IEnumerable<ICoordinate> Environment => new NullCoordinate[] { };

        private NullCoordinate()
        {

        }

        private static NullCoordinate instance = null;
    }
}