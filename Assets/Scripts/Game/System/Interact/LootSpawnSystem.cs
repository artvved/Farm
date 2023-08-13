using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.Service;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace DefaultNamespace.Game.System.Interact
{
    public class LootSpawnSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsCustomInject<Fabric> fabric = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

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
                var culture = poolCulture.Value.Get(target);
                var cultureType= culture.CultureType;
                fabric.Value.InstantiateLoot(cultureType,cultureView.LootSpawnPlace.position);
                //drop random loot
                if (CheckLootByChance(culture))
                {
                    fabric.Value.InstantiateLoot(cultureType,cultureView.LootSpawnPlace.position);
                }
            }
        }

        private bool CheckLootByChance(in Culture culture)
        {
            var val = Random.Range(0f, 100f);
            if (val<=culture.MultChance)
            {
                return true;
            }

            return false;
        }








    }
}