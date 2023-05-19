using System.Collections.Generic;
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
        
        private CultureDataService cultureDataService;

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

        public Fabric(EcsWorld world, StaticData staticData, SceneData sceneData,CultureDataService cultureDataService)
        {
            this.world = world;
            this.staticData = staticData;
            this.sceneData = sceneData;
            this.cultureDataService = cultureDataService;

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


        public int InitEmptyFarm(FarmView view)
        {
            var entity = InitEntityWithView(view);
            ref var farmStats = ref poolFarmStats.Add(entity);
            
            farmStats.CurrentCulture = CultureType.NONE;

            return entity;
        }

        public void InstantiateCultureToFarm(int farm,CultureType cultureType)
        {
            var view = (FarmView)poolBaseView.Get(farm).Value;
            var cultureData = cultureDataService.GetData(cultureType);
            
            ref var farmStats = ref poolFarmStats.Get(farm);
            farmStats.CurrentCulture = cultureData.CultureType;
            farmStats.CultureCoins = cultureData.Coins;
            farmStats.GrowthSpeedK = 1;
            farmStats.MultChance = 50;
            
            foreach (var transform in view.CulturePlaces)
            {
                InstantiateCulture(cultureData,transform,farmStats,farm);
            }
        }


        private int InstantiateCulture(CultureData cultureData,Transform transform,FarmStats farmStats,int farm)
        {
            var entity = InstantiateEntityWithView(cultureData.Prefab,transform.position);
            poolBaseView.Get(entity).Value.transform.parent = transform;

            poolCulture.Add(entity).Farm=farm;

            poolTick.Add(entity).FinalTime = cultureData.GrowthTime/farmStats.GrowthSpeedK;
            return entity;
        }

        public int InstantiateLoot(FarmStats stats,Vector3 pos)
        {
            var entity = InstantiateEntityWithView(cultureDataService.GetLootPrefab(stats.CurrentCulture), pos);
            poolCoins.Add(entity).Value = stats.CultureCoins;
            poolLoot.Add(entity);
            poolLifetime.Add(entity).Value = 3;
            return entity;
        }
        

        public int InstantiatePlayer()
        {
            var data = staticData.playerUnitData;
            var entity = InstantiateEntityWithView(data.Prefab, Vector3.zero);
            poolPlayer.Add(entity);
            poolDirection.Add(entity).Value=new Vector3(0,0,1);
            
            ref var playerStatsComponent = ref poolPlayerStats.Add(entity);
            playerStatsComponent.MaxCapacity = staticData.playerData.MaxCapacity;
            playerStatsComponent.MaxSpeed = data.MaxSpeed;
            
            ref var attacking = ref poolAttacking.Add(entity);
            attacking.Damage = data.Damage;
            attacking.AttackPeriod = data.AttackPeriod;


            ref var health =ref poolHealth.Add(entity);
            health.Hp = health.MaxHp = data.MaxHp;
            poolDefence.Add(entity).Value = data.Defence;

            poolCoins.Add(entity).Value =  data.Coins;

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
            poolCoins.Add(entity).Value = unitData.Coins;
            ref var health =ref poolHealth.Add(entity);
            health.Hp = health.MaxHp = unitData.MaxHp;
            poolDefence.Add(entity).Value = unitData.Defence;

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