﻿using Terminal.Gui;

namespace Pathfinding.ConsoleApp.View.ButtonsFrameViews
{
    internal sealed partial class SaveGraphButton : Button
    {
        private void Initialize()
        {
            X = Pos.Percent(25);
            Y = 0;
            Width = Dim.Percent(25);
            Text = "Save";
        }
    }
}
