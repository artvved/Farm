using System;
using System.Collections.Generic;
using DefaultNamespace;

namespace Game.Component
{
    [Serializable]
    public struct FarmStats
    {
        public CultureType CurrentCulture;

        public int GrowthSpeedLevel;
        public int MultChanceLevel;
        [NonSerialized]
        public List<int> CultureEntities;
    }
}