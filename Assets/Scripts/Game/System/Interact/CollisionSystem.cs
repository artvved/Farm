using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using LeoEcsPhysics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

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
        private readonly EcsPoolInject<CoinsChangedEventComponent> poolMoneyEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<FarmUIUpdateEventComponent> poolFarmUIEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<DamageEvent> poolDamageEvent = Idents.EVENT_WORLD;

        private readonly EcsPoolInject<PlayerStats> poolPlayerStats = default;
       

        private readonly EcsPoolInject<Lifetime> poolLifetime = default;

        private readonly EcsPoolInject<Coins> poolMoney = default;
        private readonly EcsPoolInject<Loot> poolLoot = default;
        private readonly EcsPoolInject<DeadTag> poolDead = default;
        private readonly EcsPoolInject<Attacking> poolAttacking = default;
        

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;


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

                var senderGameObject = enterEvent.senderGameObject.gameObject;
                var colliderGameObject = enterEvent.collider.gameObject;
                if (colliderGameObject == null || senderGameObject == null)
                    continue;
                
                //mb problems with null (fixed by adding 2nd destroySystem)

                if (senderGameObject.CompareTag("Culture") && colliderGameObject.CompareTag("Weapon"))
                {
                    Harvest(senderGameObject);
                    continue;
                }

                if (senderGameObject.CompareTag("Loot") && colliderGameObject.CompareTag("Player"))
                {
                    PickupLoot(senderGameObject, colliderGameObject);
                    continue;
                }

                if (senderGameObject.CompareTag("Zone") && colliderGameObject.CompareTag("Player"))
                {
                    InteractZone(senderGameObject, colliderGameObject);
                    continue;
                }


                ////battle
                if (senderGameObject.CompareTag("Enemy") && colliderGameObject.CompareTag("Weapon"))
                {
                    HitEnemy(senderGameObject, colliderGameObject);
                    continue;
                }
            }
        }

        private void InteractZone(GameObject senderGameObject, GameObject colliderGameObject)
        {
            int collider = colliderGameObject.GetComponent<BaseView>().Entity;
            var zoneView = senderGameObject.GetComponent<ZoneView>();
            var zoneType = zoneView.ZoneType;

            ref var coins = ref poolMoney.Value.Get(collider);
            if (zoneType == ZoneType.FARM)
            {
                poolFarmUIEvent.NewEntity(out int eventEnt).FarmEntity=zoneView.Entity;
            }
            else if (zoneType == ZoneType.SHOP)
            {
                ref var playerStats = ref poolPlayerStats.Value.Get(collider);

                colliderGameObject.GetComponent<PlayerView>().StackView
                    .ReleaseAll(((ShopZoneView) zoneView).ItemTarget);

                foreach (var lootEntity in playerStats.StackLootEntities)
                {
                    var cultureType = poolLoot.Value.Get(lootEntity).CultureType;
                    var cultureCoins = staticData.Value.Cultures[cultureType].Culture.Coins;
                    coins.Value += cultureCoins;
                }

                poolMoneyEvent.NewEntity(out int eventEnt);
                playerStats.StackLootEntities.Clear();
            }
        }

        private void Harvest(GameObject senderGameObject)
        {
            int sender = senderGameObject.GetComponent<BaseView>().Entity;

            poolEvent.NewEntity(out int newEnt).Target = world.PackEntity(sender);
        }

        private void PickupLoot(GameObject senderGameObject, GameObject colliderGameObject)
        {
            int sender = senderGameObject.GetComponent<BaseView>().Entity;
            int collider = colliderGameObject.GetComponent<BaseView>().Entity;

            ref var playerStats = ref poolPlayerStats.Value.Get(collider);
            if (playerStats.StackLootEntities.Count >= playerStats.MaxCapacity)
                return;

            poolLifetime.Value.Del(sender);
            playerStats.StackLootEntities.Add(sender);
            colliderGameObject.GetComponent<PlayerView>().StackView.AddItem(senderGameObject.GetComponent<LootView>());
        }

        private void HitEnemy(GameObject senderGameObject, GameObject colliderGameObject)
        {
            int sender = senderGameObject.GetComponent<BaseView>().Entity;
            int collider = colliderGameObject.GetComponent<BaseView>().Entity;

            ref var damageEvent = ref poolDamageEvent.NewEntity(out int newEnt);
            damageEvent.Target = world.PackEntity(sender);
            damageEvent.Damage = poolAttacking.Value.Get(collider).Damage;
            //tmp
            if (!poolDead.Value.Has(collider))
            {
                poolDead.Value.Add(collider);
            }
        }
    }
}