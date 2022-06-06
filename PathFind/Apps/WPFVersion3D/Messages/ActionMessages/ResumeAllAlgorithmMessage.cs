﻿using WPFVersion3D.ViewModel;

namespace WPFVersion3D.Messages.ActionMessages
{
    internal sealed class ResumeAllAlgorithmMessage : BaseAlgorithmsExecutionMessage
    {
        protected override void Execute(AlgorithmViewModel model)
        {
            model.Resume();
        }
    }
}