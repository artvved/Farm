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
    public class CultureSpawnTickSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private readonly EcsPoolInject<Tick> tickPool = default;
        private readonly EcsPoolInject<Harvested> harvestedPool = default;
        private readonly EcsPoolInject<BaseViewComponent> viewPool = default;
        

        private EcsFilter filterTick;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filterTick = world.Filter<Tick>().Inc<Culture>().Inc<Harvested>().End();
         
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterTick)
            {
                
                ref var component = ref tickPool.Value.Get(entity);

                if (component.CurrentTime >= component.FinalTime)
                {
                    harvestedPool.Value.Del(entity);
                    var cultureView = (CultureView)viewPool.Value.Get(entity).Value;
                    cultureView.Grow();
                }

                component.CurrentTime += Time.deltaTime;
            }
        }
    }
}