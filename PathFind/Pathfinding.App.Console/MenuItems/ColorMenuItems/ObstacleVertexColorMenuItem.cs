﻿using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Settings;

namespace Pathfinding.App.Console.MenuItems.ColorMenuItems
{
    [LowPriority]
    internal sealed class ObstacleVertexColorMenuItem : ColorsMenuItem
    {
        protected override string SettingKey { get; } = nameof(Colours.ObstacleColor);

        public ObstacleVertexColorMenuItem(IInput<int> intInput)
            : base(intInput)
        {
        }

        public override string ToString()
        {
            return Languages.ObstacleColor;
        }
    }
}