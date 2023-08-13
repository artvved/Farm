using System;
using System.Collections.Generic;
using Game.Component;

namespace DefaultNamespace
{
    [Serializable]
    public class FirstLaunchData
    {
        public int Money;
        public List<FarmStats> FarmStats=new();
    }
}