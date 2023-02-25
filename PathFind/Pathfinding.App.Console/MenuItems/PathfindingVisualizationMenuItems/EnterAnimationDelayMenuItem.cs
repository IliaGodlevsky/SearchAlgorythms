﻿using GalaSoft.MvvmLight.Messaging;
using Pathfinding.App.Console.Extensions;
using Pathfinding.App.Console.Interface;
using Pathfinding.App.Console.Localization;
using Pathfinding.App.Console.MenuItems.MenuItemPriority;
using Pathfinding.App.Console.Messages;
using Shared.Primitives.ValueRange;
using System;

namespace Pathfinding.App.Console.MenuItems.PathfindingVisualizationMenuItems
{
    [HighPriority]
    internal sealed class EnterAnimationDelayMenuItem : IConditionedMenuItem, ICanRecieveMessage
    {
        private readonly IInput<TimeSpan> spanInput;
        private readonly IMessenger messenger;

        private InclusiveValueRange<TimeSpan> DelayRange { get; }

        private bool IsVisualizationApplied { get; set; }

        public EnterAnimationDelayMenuItem(IInput<TimeSpan> spanInput, IMessenger messenger)
        {
            DelayRange = Constants.AlgorithmDelayTimeValueRange;
            this.spanInput = spanInput;
            this.messenger = messenger;
        }

        public void Execute()
        {
            using (Cursor.UseCurrentPositionWithClean())
            {
                var delay = spanInput.Input(Languages.DelayTimeInputMsg, DelayRange);
                messenger.Send(new AnimationDelayMessage(delay));
            }
        }

        public bool CanBeExecuted() => IsVisualizationApplied;

        private void OnVisualizationApplied(ApplyVisualizationMessage message)
        {
            IsVisualizationApplied = message.IsApplied;
        }

        public override string ToString()
        {
            return Languages.EnterAnimationDelay;
        }

        public void RegisterHanlders(IMessenger messenger)
        {
            messenger.Register<ApplyVisualizationMessage>(this, OnVisualizationApplied);
        }
    }
}
