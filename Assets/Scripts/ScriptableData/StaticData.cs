using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DefaultNamespace;
using Game.Mono;
using Game.UI;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class StaticData : ScriptableObject
    {
        public int MaxPictureSize;
        
        public PlayerData playerData;
        public UnitData playerUnitData;
        public BulletData BulletData;
        
        //
        public int LifestealPercent { get; set; }

        public UnitData[] Enemies;

        public SerializedDictionary<CultureType,CultureData> Cultures;
        public float[] GrowthSpeedKProgression;
        public int[] GrowthSpeedKCostProgression;
        public float[] MultKProgression;
        public int[] MultKCostProgression;

        [Header("FirstLaunch")] [SerializeField]
        public FirstLaunchData FirstLaunchData;
    }
}