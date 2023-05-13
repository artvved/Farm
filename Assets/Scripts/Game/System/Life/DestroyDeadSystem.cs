using Game.Component;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System.Timing
{
    public class DestroyDeadSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<BaseViewComponent> viewPool = default;

        private EcsFilter deadFilter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            deadFilter = world.Filter<DeadTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in deadFilter)
            {
                if (viewPool.Value.Has(unit))
                {
                    GameObject.Destroy(viewPool.Value.Get(unit).Value.gameObject);
                }

                world.DelEntity(unit);
            }
        }
    }
}