﻿using System;

namespace Common.Attributes
{
    /// <summary>
    /// Indicates that a class should be ignored when fetching classes from an assembly
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FilterableAttribute : Attribute
    {

    }
}