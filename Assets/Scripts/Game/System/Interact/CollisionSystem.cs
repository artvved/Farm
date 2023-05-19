using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace DefaultNamespace.Game.System.Interact
{
    public class CollisionSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<OnTriggerEnterEvent> poolTriggerEnter = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<OnTriggerExitEvent> poolTriggerExit = Idents.EVENT_WORLD;

        private readonly EcsPoolInject<HarvestEvent> poolEvent = Idents.EVENT_WORLD;

        //tmp
        private readonly EcsPoolInject<CoinsChangedEventComponent> poolCoinsEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<DamageEvent> poolDamageEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<Coins> poolCoins = default;

        private readonly EcsPoolInject<DeadTag> poolDead = default;
        private readonly EcsPoolInject<PlayerTag> poolPlayer = default;
        private readonly EcsPoolInject<DamagingBody> poolDamaging = default;
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<Loot> poolLoot = default;
        private readonly EcsPoolInject<Enemy> poolEnemy = default;
        private readonly EcsPoolInject<Attacking> poolAttacking = default;

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


                //harvest
                if (IsCulture(sender) && IsDamaging(collider))
                {
                    poolEvent.NewEntity(out int newEnt).Target = world.PackEntity(sender);
                    continue;
                }
                
                //bullet
                if (IsEnemy(sender) && IsDamaging(collider))
                {
                    ref var damageEvent = ref poolDamageEvent.NewEntity(out int newEnt);
                    damageEvent.Target = world.PackEntity(sender);
                    damageEvent.Damage = poolAttacking.Value.Get(collider).Damage;
                    //tmp
                    if (!poolDead.Value.Has(collider))
                    {
                        poolDead.Value.Add(collider);
                    }
                  
                    continue;
                }

                if (IsLoot(sender) && IsPlayer(collider))
                {
                    //tmp
                    poolDead.Value.Add(sender);
                    ref var coins = ref poolCoins.Value.Get(collider);
                    coins.Value++;
                    poolCoinsEvent.NewEntity(out int newEnt);
                    //poolEvent.NewEntity(out int newEnt).Target=world.PackEntity(sender);
                    continue;
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

        private bool IsLoot(int ent)
        {
            return poolLoot.Value.Has(ent);
        }
        
        private bool IsEnemy(int ent)
        {
            return poolEnemy.Value.Has(ent);
        }
    }
}