using DefaultNamespace;
using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class CultureData : ScriptableObject
    {
        public CultureView Prefab;
        public LootView LootPrefab;
        public float GrowthTime;
        public int Coins;
        public CultureType CultureType;

    }
}