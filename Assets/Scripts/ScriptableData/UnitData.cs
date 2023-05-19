using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class UnitData : ScriptableObject
    {
        public BaseView Prefab;
        public float MaxSpeed;
        public int MaxHp;
        public int Defence;
        public int Damage;
        public float AttackPeriod;
        
        public int Coins;
       
    }
}