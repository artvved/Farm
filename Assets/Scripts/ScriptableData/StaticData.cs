using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public PlayerData playerData;
        public UnitData playerUnitData;
        public BulletData BulletData;
        
        public UnitData[] Enemies;
        
        public CultureData[] Cultures;
        
        //
        public int LifestealPercent { get; set; }
    }
}