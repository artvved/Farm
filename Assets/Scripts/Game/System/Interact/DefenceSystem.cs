using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace DefaultNamespace.Game.System.Interact
{
    public class DefenceSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<DamageEvent> poolDamageEvent = Idents.EVENT_WORLD;
        
        private readonly EcsPoolInject<Defence> poolDefence = default;


      
        private EcsFilter filterDamageEvent;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            filterDamageEvent = eventWorld.Filter<DamageEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in filterDamageEvent)
            {
                if(!poolDamageEvent.Value.Get(e).Target.Unpack(world,out int target))
                    continue;
                ref var damage = ref poolDamageEvent.Value.Get(e).Damage;
                if (poolDefence.Value.Has(target))
                {
                    var defence = poolDefence.Value.Get(target).Value;
                    damage-=(int)(damage * defence / 100f);
                }
            }

            
        }


       
    }
}