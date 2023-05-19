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
    public class EnemySpawnTickSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<Tick> tickPool = default;
        private readonly EcsPoolInject<Active> poolActive = default;
        private readonly EcsCustomInject<Fabric> fabric = default;
        private readonly EcsCustomInject<StaticData> data = default;
        private readonly EcsCustomInject<SceneData> sceneData = default;


        private EcsFilter filterActiveSpawner;
        private EcsFilter filterSwitchEvent;
        private int spawner;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            spawner = world.NewEntity();
            world.GetPool<EnemySpawner>().Add(spawner);
            ref var tick = ref tickPool.Value.Add(spawner);
            tick.CurrentTime=tick.FinalTime=20;
            filterSwitchEvent = eventWorld.Filter<SwitchEvent>().End();
            filterActiveSpawner = world.Filter<Tick>().Inc<Active>().Inc<EnemySpawner>().End();
         
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in filterSwitchEvent)
            {
                if (poolActive.Value.Has(spawner))
                {
                    poolActive.Value.Del(spawner);
                }
                else
                {
                    poolActive.Value.Add(spawner);
                }
            }
            foreach (var entity in filterActiveSpawner)
            {
                ref var tick = ref tickPool.Value.Get(entity);

                if (tick.CurrentTime >= tick.FinalTime)
                {
                    var position = sceneData.Value.EnemySpawnPlace.position;
                    for (int i = 0; i < 3; i++)
                    {
                        fabric.Value.InstantiateEnemy(data.Value.Enemies[0],position+new Vector3(i+1,0,0));
                    }
                   
                    tick.CurrentTime = 0;
                }
                tick.CurrentTime += Time.deltaTime;
            }
        }
    }
}