using System;

namespace Game.Component
{
    [Serializable]
    public struct Culture
    {
        public CultureType CultureType;
        public float GrowthTime;
        public float MultChance;
        public int Coins;
    }
}