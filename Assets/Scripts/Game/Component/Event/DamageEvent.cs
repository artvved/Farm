using Leopotam.EcsLite;

namespace Game.Component
{
    public struct DamageEvent
    {
        public EcsPackedEntity Target;
        public int Damage;
    }
}