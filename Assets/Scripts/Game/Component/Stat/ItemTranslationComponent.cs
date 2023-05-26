using Leopotam.EcsLite;

namespace Game.Component
{
    public struct ItemTranslationComponent
    {
        public EcsPackedEntity Target;
        public bool IsPutting;
    }
}