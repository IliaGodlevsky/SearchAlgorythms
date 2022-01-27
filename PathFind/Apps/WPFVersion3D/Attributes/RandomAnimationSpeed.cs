﻿using Autofac;
using Random.Extensions;
using Random.Interface;
using ValueRange;
using WPFVersion3D.DependencyInjection;
using WPFVersion3D.Interface;

namespace WPFVersion3D.Attributes
{
    internal sealed class RandomAnimationSpeed : BaseAnimationSpeed, IAnimationSpeed
    {
        public RandomAnimationSpeed(double from, double to)
        {
            range = new InclusiveValueRange<double>(to, from);
            random = DI.Container.Resolve<IRandom>();
        }

        public override double Milliseconds => random.NextDouble(range);

        private readonly InclusiveValueRange<double> range;
        private readonly IRandom random;
    }
}