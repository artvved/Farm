using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace DefaultNamespace.Game.System.Interact
{
    public class CollisionSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<OnTriggerEnterEvent> poolTriggerEnter = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<OnTriggerExitEvent> poolTriggerExit = Idents.EVENT_WORLD;
        
        private readonly EcsPoolInject<HarvestEvent> poolEvent = Idents.EVENT_WORLD;
  
        private readonly EcsPoolInject<PlayerTag> poolPlayer = default;
        private readonly EcsPoolInject<Damaging> poolDamaging = default;
        private readonly EcsPoolInject<Culture> poolCulture = default;


        private EcsFilter enterFilter;
        private EcsFilter exitFilter;
   

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);

            enterFilter = eventWorld.Filter<OnTriggerEnterEvent>().End();
            exitFilter = eventWorld.Filter<OnTriggerExitEvent>().End();
    
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var exitEnt in exitFilter)
            {
                var triggerExitEvent = poolTriggerExit.Value.Get(exitEnt);

                var senderView = triggerExitEvent.senderGameObject.gameObject.GetComponent<BaseView>();
                var colliderView = triggerExitEvent.collider.gameObject.GetComponent<BaseView>();

                if (senderView == null)
                    continue;
                if (colliderView == null)
                    continue;

                int sender = senderView.Entity;
                int collider = colliderView.Entity;

              
              
            }
            
            foreach (var enterEnt in enterFilter)
            {
                var enterEvent = poolTriggerEnter.Value.Get(enterEnt);

                var senderView = enterEvent.senderGameObject.gameObject.GetComponent<BaseView>();
                var colliderView = enterEvent.collider.gameObject.GetComponent<BaseView>();

                if (senderView == null)
                    continue;
                if (colliderView == null)
                    continue;

                int sender = senderView.Entity;
                int collider = colliderView.Entity;

              
                if (IsCulture(sender) && IsDamaging(collider))
                {
                    poolEvent.NewEntity(out int newEnt).Target=world.PackEntity(sender);
                }
                
            }

           
        }

   
        
        
        private bool IsPlayer(int ent)
        {
            return poolPlayer.Value.Has(ent);
        }
        private bool IsCulture(int ent)
        {
            return poolCulture.Value.Has(ent);
        }
        private bool IsDamaging(int ent)
        {
            return poolDamaging.Value.Has(ent);
        }

        
      

       
    }
}