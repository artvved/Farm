using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.Service;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace DefaultNamespace.Game.System.Interact
{
    public class LootSpawnSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsCustomInject<Fabric> fabric = default;

      
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        private readonly EcsPoolInject<FarmStats> poolFarm = default;
        private readonly EcsPoolInject<HarvestEvent> poolEvent = Idents.EVENT_WORLD;
        
        private EcsFilter filter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            filter = eventWorld.Filter<HarvestEvent>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in filter)
            {
                poolEvent.Value.Get(entity).Target.Unpack(world, out int target);
                
                var cultureView = (CultureView)poolView.Value.Get(target).Value;

                //drop loot
                var farm = poolCulture.Value.Get(target).Farm;
                var stats = poolFarm.Value.Get(farm);
                fabric.Value.InstantiateLoot(stats,cultureView.LootSpawnPlace.position);
                //drop random loot
                if (CheckLootByChance(stats))
                {
                    fabric.Value.InstantiateLoot(stats,cultureView.LootSpawnPlace.position);
                }
            }
        }

        private bool CheckLootByChance(in FarmStats farmStats)
        {
            var val = Random.Range(0f, 100f);
            if (val<=farmStats.MultChance)
            {
                return true;
            }

            return false;
        }








    }
}