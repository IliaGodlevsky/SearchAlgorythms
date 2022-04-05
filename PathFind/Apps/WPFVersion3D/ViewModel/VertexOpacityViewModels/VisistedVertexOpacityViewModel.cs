﻿using System.Windows.Media;
using WPFVersion3D.Model;

namespace WPFVersion3D.ViewModel.VertexOpacityViewModels
{
    internal class VisistedVertexOpacityViewModel : BaseVertexOpacityViewModel
    {
        protected override Brush Color => VertexVisualization.VisitedVertexBrush;
    }
}