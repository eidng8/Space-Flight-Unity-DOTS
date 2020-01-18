﻿using Unity.Entities;
using Unity.Mathematics;

namespace eidng8.SpaceFlight.Components
{
    /// <summary>
    /// Anything that can move should has this component attached to it.
    /// </summary>
    public struct DefenseComponent : IComponentData
    {
        public float armor;
        public float shield;
    }
}
