using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System.Timing
{
    public class LifetimeSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<Lifetime> poolLifetime = default;
        private readonly EcsPoolInject<DeadTag> poolDead = default;
     

        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<Lifetime>().End();
         
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                ref var component = ref poolLifetime.Value.Get(entity);

                if (component.Value < 0)
                {
                    poolDead.Value.Add(entity);
                }

                component.Value -= Time.deltaTime;
            }
        }
    }
}