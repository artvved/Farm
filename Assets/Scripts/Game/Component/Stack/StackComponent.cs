using Leopotam.EcsLite;
using UnityEngine;

namespace Game.Component
{
    public struct StackComponent
    {
        public int MaxCapacity;
        public int CurrCapacity;
        //public Transform Root;
        public Transform[] Places;
        public EcsPackedEntity[] Entities;
    }
}