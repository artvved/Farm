using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace DefaultNamespace.Game.System.Interact
{
    public class HarvestSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<Tick> poolTick = default;
   
        private readonly EcsPoolInject<Harvested> poolHarvested = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
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
                cultureView.Harvest();
                poolTick.Value.Get(target).CurrentTime = 0;
                poolHarvested.Value.Add(target);
                //drop loot
            }
        }

   
        
        
        
      

       
    }
}