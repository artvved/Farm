using Game.Mono;
using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class BulletData : ScriptableObject
    {
        public BaseView BulletPrefab;
        public float Speed;
    }
}