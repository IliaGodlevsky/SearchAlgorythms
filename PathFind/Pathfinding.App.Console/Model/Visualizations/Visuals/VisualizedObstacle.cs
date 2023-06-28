﻿using Pathfinding.App.Console.Settings;

namespace Pathfinding.App.Console.Model.Visualizations.Visuals
{
    internal sealed class VisualizedObstacle : VisualizedVertices
    {
        protected override string SettingKey { get; } = nameof(Colours.ObstacleColor);
    }
}