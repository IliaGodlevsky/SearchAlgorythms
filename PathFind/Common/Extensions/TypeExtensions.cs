﻿using System;
using System.Reflection;

namespace Common.Extensions
{
    public static class TypeExtensions
    {
        public static Assembly GetAssembly(this Type self)
        {
            return Assembly.Load(self.Assembly.GetName());
        }

        public static ConstructorInfo GetConstructor(this Type self, params Type[] parametres)
        {
            return self.GetConstructor(parametres);
        }

        public static TAttribute GetAttribute<TAttribute>(this Type self, bool inherit = false) where TAttribute : Attribute
        {
            return (TAttribute)Attribute.GetCustomAttribute(self, typeof(TAttribute), inherit);
        }
    }
}