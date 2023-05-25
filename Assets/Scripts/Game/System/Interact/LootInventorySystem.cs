using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace DefaultNamespace.Game.System.Interact
{
    public class LootInventorySystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<LootInventoryEvent> poolEvent = Idents.EVENT_WORLD;
  
        private readonly EcsPoolInject<Inventory> poolInv = default;
        
        private EcsFilter filterEvent;
        private EcsFilter filterPlayer;
        
        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            
            filterEvent = eventWorld.Filter<LootInventoryEvent>().End();
            filterPlayer = world.Filter<PlayerTag>().Inc<Inventory>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in filterEvent)
            {
                foreach (var player in filterPlayer)
                {
                    if (!poolEvent.Value.Get(e).Target.Unpack(world, out int from))
                        continue;

                    var fromDictionary = poolInv.Value.Get(from).Value;
                    ref var toDictionary = ref poolInv.Value.Get(player).Value;
                
                    foreach (var kv in fromDictionary)
                    {
                        toDictionary[kv.Key] += kv.Value;
                        Debug.Log(toDictionary[kv.Key]);
                    }
                }
              
                
            }
        }
    }
}