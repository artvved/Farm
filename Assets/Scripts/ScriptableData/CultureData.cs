using DefaultNamespace;
using Game.Component;
using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class CultureData : ScriptableObject
    {
        public Sprite Icon;
        public CultureView Prefab;
        public LootView LootPrefab;
        public Culture Culture;

    }
}