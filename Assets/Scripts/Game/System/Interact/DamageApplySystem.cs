using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace DefaultNamespace.Game.System.Interact
{
    public class DamageApplySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<DamageEvent> poolDamageEvent = Idents.EVENT_WORLD;
        
        private readonly EcsPoolInject<Health> poolHealth = default;
        private readonly EcsPoolInject<DeadTag> poolDead = default;


      
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
                var damage = poolDamageEvent.Value.Get(e).Damage;
                ref var hp = ref poolHealth.Value.Get(target).Hp;
                hp -= damage;

                if (hp<=0)
                {
                    if (!poolDead.Value.Has(target))
                    {
                        poolDead.Value.Add(target);
                    }
                }
            }

            
        }


       
    }
}