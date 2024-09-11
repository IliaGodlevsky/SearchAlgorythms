﻿using Autofac.Features.AttributeFilters;
using Pathfinding.ConsoleApp.Injection;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;
using ReactiveMarbles.ObservableEvents;
using System.Reactive.Linq;
using System;

namespace Pathfinding.ConsoleApp.View.RightPanelViews.Runs.CreateRun
{
    internal sealed partial class AlgorithmsListView : FrameView
    {
        public AlgorithmsListView([KeyFilter(KeyFilters.AlgorithmsListView)]IEnumerable<Terminal.Gui.View> children)
        {
            Initialize();
            var names = children.ToDictionary(x => x.Text, x => x);
            algorithms.RadioLabels = names.Keys.ToArray();
            algorithms.Events().SelectedItemChanged
                .Where(x => x.SelectedItem > -1)
                .Do(x =>
                {
                    var key = algorithms.RadioLabels[x.SelectedItem];
                    var element = names[key];
                    element.OnMouseEvent(new Terminal.Gui.MouseEvent() { Flags = MouseFlags.Button1Clicked});
                })
                .Subscribe();
            //algorithms.Add(children.ToArray());
        }
    }
}
