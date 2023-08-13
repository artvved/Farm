using System;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace Game.Service
{
    public class Fabric
    {
        private EcsWorld world;

        private StaticData staticData;
        private SceneData sceneData;

        private EcsPool<PlayerTag> poolPlayer;
        private EcsPool<PlayerStats> poolPlayerStats;
        private EcsPool<BaseViewComponent> poolBaseView;
        private EcsPool<FarmStats> poolFarmStats;
        private EcsPool<Culture> poolCulture;
        private EcsPool<Tick> poolTick;
        private EcsPool<DamagingBody> poolDamaging;
        private EcsPool<Loot> poolLoot;
        private EcsPool<Coins> poolCoins;
        private EcsPool<Lifetime> poolLifetime;
        private EcsPool<Enemy> poolEnemy;
        private EcsPool<Attacking> poolAttacking;
        private EcsPool<Health> poolHealth;
        private EcsPool<Defence> poolDefence;
        private EcsPool<Direction> poolDirection;
        private EcsPool<Speed> poolSpeed;
        private EcsPool<Inventory> poolInv;

        public Fabric(EcsWorld world, StaticData staticData, SceneData sceneData)
        {
            this.world = world;
            this.staticData = staticData;
            this.sceneData = sceneData;
         
            poolPlayer = world.GetPool<PlayerTag>();
            poolPlayerStats = world.GetPool<PlayerStats>();
            poolFarmStats = world.GetPool<FarmStats>();
            poolBaseView = world.GetPool<BaseViewComponent>();
            poolCulture = world.GetPool<Culture>();
            poolTick = world.GetPool<Tick>();
            poolDamaging = world.GetPool<DamagingBody>();
            poolLoot = world.GetPool<Loot>();
            poolCoins = world.GetPool<Coins>();
            poolLifetime = world.GetPool<Lifetime>();
            poolEnemy = world.GetPool<Enemy>();
            poolAttacking = world.GetPool<Attacking>();
            poolHealth = world.GetPool<Health>();
            poolDefence = world.GetPool<Defence>();
            poolDirection = world.GetPool<Direction>();
            poolSpeed = world.GetPool<Speed>();
            poolInv = world.GetPool<Inventory>();
        }


        private int InstantiateEntityWithView(BaseView prefab, Vector3 position)
        {
            var view = GameObject.Instantiate(prefab);
            view.transform.position = position;
            int entity = world.NewEntity();
            view.Entity = entity;

            poolBaseView.Add(entity).Value = view;

            return entity;
        }

        private int InitEntityWithView(BaseView view)
        {
            int entity = world.NewEntity();
            view.Entity = entity;

            poolBaseView.Add(entity).Value = view;

            return entity;
        }


        public int InitEmptyFarm(FarmView view,FarmStats farmStatsInit)
        {
            var entity = InitEntityWithView(view);
            view.zone.Entity = entity;
            ref var farmStats = ref poolFarmStats.Add(entity);
            
            farmStats.CurrentCulture = farmStatsInit.CurrentCulture;
            farmStats.GrowthSpeedLevel = farmStatsInit.GrowthSpeedLevel;
            farmStats.MultChanceLevel =farmStatsInit.MultChanceLevel;
            
            return entity;
        }

        public void InstantiateCultureToFarm(int farm,CultureData cultureData)
        {
            var view = (FarmView)poolBaseView.Get(farm).Value;

            ref var farmStats = ref poolFarmStats.Get(farm);
            farmStats.CurrentCulture = cultureData.Culture.CultureType;
            farmStats.CultureEntities = new List<int>();
            foreach (var transform in view.CulturePlaces)
            {
                var culture = InstantiateCulture(cultureData,transform,farmStats);
                farmStats.CultureEntities.Add(culture);
            }
        }


        private int InstantiateCulture(CultureData cultureData,Transform transform,FarmStats farmStats)
        {
            var entity = InstantiateEntityWithView(cultureData.Prefab,transform.position);
            poolBaseView.Get(entity).Value.transform.parent = transform;

            ref var culture = ref poolCulture.Add(entity);
            culture.CultureType = cultureData.Culture.CultureType;
            culture.GrowthTime = cultureData.Culture.GrowthTime;
            culture.MultChance = cultureData.Culture.MultChance*staticData.MultKProgression[farmStats.MultChanceLevel];
            culture.Coins = cultureData.Culture.Coins;

            poolTick.Add(entity).FinalTime = cultureData.Culture.GrowthTime*staticData.GrowthSpeedKProgression[farmStats.GrowthSpeedLevel];
            return entity;
        }

        public int InstantiateLoot(CultureType cultureType,Vector3 pos)
        {
            var entity = InstantiateEntityWithView(staticData.Cultures[cultureType].LootPrefab, pos);
           // poolCoins.Add(entity).Value = stats.CultureCoins;
            poolLoot.Add(entity).CultureType=cultureType;
            poolLifetime.Add(entity).Value = 3;
            return entity;
        }
        

        public int InstantiatePlayer(bool isFirstTime,int coins)
        {
            var data = staticData.playerUnitData;
            var entity = InstantiateEntityWithView(data.Prefab, Vector3.zero);
            poolPlayer.Add(entity);
            poolDirection.Add(entity).Value=new Vector3(0,0,1);
            
            ref var playerStats = ref poolPlayerStats.Add(entity);
            playerStats.MaxCapacity = staticData.playerData.MaxCapacity;
            playerStats.MaxSpeed = data.MaxSpeed;
            playerStats.IsFirstTime = isFirstTime;
            playerStats.StackLootEntities = new List<int>();
            
            ref var attacking = ref poolAttacking.Add(entity);
            attacking.Damage = data.Damage;
            attacking.AttackPeriod = data.AttackPeriod;


            ref var health =ref poolHealth.Add(entity);
            health.Hp = health.MaxHp = data.MaxHp;
            poolDefence.Add(entity).Value = data.Defence;

            poolCoins.Add(entity).Value =  coins;
            
            Dictionary<ItemType, int> dict = new Dictionary<ItemType, int>();
            var values = Enum.GetValues(typeof(ItemType));
            foreach (var item in values)
            {
                dict.Add((ItemType)item,0);
            }

            poolInv.Add(entity).Value = dict;

            poolTick.Add(entity).FinalTime = attacking.AttackPeriod;
            
            var playerView = (PlayerView)poolBaseView.Get(entity).Value;
            var playerViewWeapon = playerView.Weapon;

            InitWeapon(playerViewWeapon);

            return entity;
        }

        public int InitWeapon(BaseView baseView)
        {
            var weapon=InitEntityWithView(baseView);
            poolDamaging.Add(weapon);
            return weapon;
        }
        
        public int InstantiateEnemy(UnitData unitData,Vector3 pos)
        {
            var entity = InstantiateEntityWithView(unitData.Prefab, pos);
            poolEnemy.Add(entity);
            
            ref var attacking = ref poolAttacking.Add(entity);
            attacking.Damage = unitData.Damage;
            attacking.AttackPeriod = unitData.AttackPeriod;

            ref var health =ref poolHealth.Add(entity);
            health.Hp = health.MaxHp = unitData.MaxHp;
            poolDefence.Add(entity).Value = unitData.Defence;
            
            poolCoins.Add(entity).Value = unitData.Coins;
            
            poolInv.Add(entity).Value = new Dictionary<ItemType, int>()
            {
                {unitData.Drop,unitData.DropCount}
            };

            return entity;
        }

        public int InstantiateBullet(Vector3 position, Vector3 direction, int damage)
        {
            var bullet= InstantiateEntityWithView(staticData.BulletData.BulletPrefab,position);
            poolDamaging.Add(bullet);
            poolAttacking.Add(bullet).Damage = damage;
            poolDirection.Add(bullet).Value = direction;
            poolSpeed.Add(bullet).Value = staticData.BulletData.Speed;
            poolLifetime.Add(bullet).Value = 5;
            return bullet;
        }
    }
}