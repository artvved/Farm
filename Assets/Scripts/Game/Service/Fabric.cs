using System.Collections.Generic;
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
        private EcsWorld eventWorld;
        private StaticData staticData;

        
        private EcsPool<PlayerTag> playerPool;
        private EcsPool<PlayerStatsComponent> playerStatsPool;
        private EcsPool<BaseViewComponent> baseViewPool;
       
            
        public Fabric(EcsWorld world,EcsWorld eventWorld,  StaticData staticData)
        {
            this.world = world;
            this.eventWorld = eventWorld;
            this.staticData = staticData;

           
            playerPool = world.GetPool<PlayerTag>();
            playerStatsPool = world.GetPool<PlayerStatsComponent>();

            baseViewPool = world.GetPool<BaseViewComponent>();
            
        }


        private int InstantiateUnit(UnitView prefab, Vector3 position)
        {
            var view = GameObject.Instantiate(prefab);
            view.transform.position = position;
            int unitEntity = world.NewEntity();
            view.Entity = unitEntity;
            
            baseViewPool.Add(unitEntity).Value = view;

            return unitEntity;
        }

        public int InstantiatePlayer()
        {
            var playerEntity = InstantiateUnit(staticData.PlayerPrefab, Vector3.zero);
            playerPool.Add(playerEntity);
            ref var playerStatsComponent = ref playerStatsPool.Add(playerEntity);
            playerStatsComponent.MaxCapacity = staticData.PlayerStats.MaxCapacity;
            playerStatsComponent.Coins = staticData.PlayerStats.Coins;
            playerStatsComponent.MaxSpeed = staticData.PlayerStats.MaxSpeed;
           
            return playerEntity;
        }

       
    }
}