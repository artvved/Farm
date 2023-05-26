using DefaultNamespace;
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
    public class RemoveCultureSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<RemoveCultureEvent> poolEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<DeadTag> poolDead = default;
        

        private EcsFilter filterEvent;
        private EcsFilter filter;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filterEvent = eventWorld.Filter<RemoveCultureEvent>().End();
            filter = world.Filter<Culture>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filterEvent)
            {
                var farmTarget = poolEvent.Value.Get(entity).FarmTarget;
                foreach (var culture in filter)
                {
                    if(poolCulture.Value.Get(culture).Farm == farmTarget)
                    {
                        poolDead.Value.Add(culture);
                    }
                }
            }
        }
    }
}