using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace Game.Component
{
    [Serializable]
    public struct FarmStats
    {
        public CultureType CurrentCulture;

        public int GrowthSpeedLevel;
        public int MultChanceLevel;

        public Texture2D GroundPicture;
        [NonSerialized]
        public List<int> CultureEntities;
    }
}