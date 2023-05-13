using System.Collections.Generic;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;

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
        private EcsPool<Damaging> poolDamaging;


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
            poolDamaging = world.GetPool<Damaging>();
        }


        private int InstantiateEntWithView(BaseView prefab, Vector3 position)
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


        public int InitFarm(FarmView view,CultureData cultureData)
        {
            var entity = InitEntityWithView(view);
            ref var farmStats = ref poolFarmStats.Add(entity);
            farmStats.CultureCoins = cultureData.Coins;
            farmStats.CurrentCulture = cultureData.CultureType;

            foreach (var transform in view.CulturePlaces)
            {
                InstantiateCulture(cultureData,transform);
            }
            
            return entity;
        }

        public int InstantiateCulture(CultureData cultureData,Transform transform)
        {
            var entity = InstantiateEntWithView(cultureData.Prefab,transform.position);
            poolBaseView.Get(entity).Value.transform.parent = transform;
            poolCulture.Add(entity);
            poolTick.Add(entity).FinalTime = cultureData.GrowthTime;
            return entity;
        }

        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateEntWithView(staticData.PlayerPrefab, Vector3.zero);
            poolPlayer.Add(playerEntity);
            ref var playerStatsComponent = ref poolPlayerStats.Add(playerEntity);
            playerStatsComponent.MaxCapacity = staticData.playerData.MaxCapacity;
            playerStatsComponent.Coins = staticData.playerData.Coins;
            playerStatsComponent.MaxSpeed = staticData.playerData.MaxSpeed;
            
            var playerView = (PlayerView)poolBaseView.Get(playerEntity).Value;
            sceneData.Camera.Follow = playerView.transform;
            sceneData.Camera.LookAt = playerView.LookAt;
            var playerViewWeapon = playerView.Weapon;
            var weapon=InitEntityWithView(playerViewWeapon);
            poolDamaging.Add(weapon);

            return playerEntity;
        }
    }
}