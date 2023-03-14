using UnityEngine;

namespace ScriptableData
{
    [CreateAssetMenu]
    public class PlayerStats : ScriptableObject
    {
        public float MaxSpeed;
        public float MaxCapacity;
        public int Coins;
       
    }
}