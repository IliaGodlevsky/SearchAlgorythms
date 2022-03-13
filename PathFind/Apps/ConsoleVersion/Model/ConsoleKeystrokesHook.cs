﻿using ConsoleVersion.EventArguments;
using ConsoleVersion.EventHandlers;
using ConsoleVersion.Interface;
using Interruptable.EventArguments;
using Interruptable.EventHandlers;
using Interruptable.Interface;
using SingletonLib;
using System;

namespace ConsoleVersion.Model
{
    internal sealed class ConsoleKeystrokesHook
        : Singleton<ConsoleKeystrokesHook, ConsoleKeystrokesHook>, IInterruptable, IProcess, IRequireConsoleKeyInput
    {
        public event ConsoleKeyPressedEventHandler KeyPressed;
        public event ProcessEventHandler Interrupted;
        public event ProcessEventHandler Started;
        public event ProcessEventHandler Finished;

        public bool IsInProcess { get; private set; }
        public IInput<ConsoleKey> KeyInput { get; set; }

        public void Interrupt()
        {
            IsInProcess = false;
            Finished?.Invoke(this, new ProcessEventArgs());
            Interrupted?.Invoke(this, new ProcessEventArgs());
        }

        public void Start()
        {
            Started?.Invoke(this, new ProcessEventArgs());
            IsInProcess = true;
            while (IsInProcess)
            {
                var key = KeyInput.Input();
                var args = new ConsoleKeyPressedEventArgs(key);
                KeyPressed?.Invoke(this, args);
            }
        }

        private ConsoleKeystrokesHook()
        {

        }
    }
}