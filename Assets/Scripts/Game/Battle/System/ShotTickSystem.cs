using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System.Interact
{
    public class ShotTickSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private EcsCustomInject<Fabric> fabric = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        
        private  readonly EcsPoolInject<Direction> poolDirection = default;
        private  readonly EcsPoolInject<Attacking> poolAttacking = default;
        private  readonly EcsPoolInject<Tick> poolTick = default;

        private EcsFilter filterEvent;
        private EcsFilter filterShooter;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filterEvent = eventWorld.Filter<ShotEvent>().End();
            filterShooter = world.Filter<PlayerTag>().Inc<Tick>().End();
        }

        public void Run(IEcsSystems systems)
        {
            
            foreach (var shooter in filterShooter)
            {
                ref var tick = ref poolTick.Value.Get(shooter);
                if (tick.CurrentTime>=tick.FinalTime)
                {
                    foreach (var entity in filterEvent)
                    {
                        Shot(shooter);
                        tick.CurrentTime = 0;
                    }
                }
                else
                {
                    tick.CurrentTime += Time.deltaTime;
                }

            }
           
        }

        private void Shot(int shooter)
        {
            var view=(PlayerView)poolView.Value.Get(shooter).Value;
            var direction = poolDirection.Value.Get(shooter).Value;
            var damage = poolAttacking.Value.Get(shooter).Damage;
            fabric.Value.InstantiateBullet(view.transform.position+new Vector3(0,1,0), direction,damage);
        }
    }
}