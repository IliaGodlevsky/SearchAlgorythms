﻿using System;

namespace GraphLibrary.RoleChanger
{
    public interface IVertexRoleChanger
    {
        void SetStartPoint(object sender, EventArgs e);
        void SetDestinationPoint(object sender, EventArgs e);
        void ReversePolarity(object sender, EventArgs e);
        void ChangeTopText(object sender, EventArgs e);
    }
}