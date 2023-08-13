using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class LoadWorldSystem : IEcsInitSystem
    {
        private EcsWorld world;

        private readonly EcsCustomInject<Fabric> fabric = default;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private EcsPoolInject<Coins> poolMoney;
        private EcsPoolInject<PlayerStats> poolPlayer;
        private EcsPoolInject<FarmStats> poolFarm;
        private EcsPoolInject<BaseViewComponent> poolBaseView;
        

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            
            //Loading saved or default Data
            bool isFirstTime;
            int coins;
            List<FarmStats> farmStatsList = new List<FarmStats>();
            if (!PlayerPrefs.HasKey("IsFirstTime") || sceneData.Value.StartNewData)
            {
                isFirstTime = true;
                coins = staticData.Value.FirstLaunchData.Money;
                
                var statsList = staticData.Value.FirstLaunchData.FarmStats;
                
                for (int i = 0; i < sceneData.Value.Farms.Length; i++)
                {
                    var farmStat = new FarmStats();
                    farmStat.CurrentCulture = statsList[i].CurrentCulture;
                    farmStat.GrowthSpeedLevel = statsList[i].GrowthSpeedLevel;
                    farmStat.MultChanceLevel = statsList[i].MultChanceLevel;

                    farmStatsList.Add(farmStat);
                }
            }
            else
            {
                isFirstTime = (PlayerPrefs.GetInt("IsFirstTime")) == 0;
                coins = PlayerPrefs.GetInt("Money");

                for (int i = 0; i < sceneData.Value.Farms.Length; i++)
                {
                    var farmStat = new FarmStats();
                    farmStat.CurrentCulture = (CultureType) PlayerPrefs.GetInt($"Farm{i}_CultureType");
                    farmStat.GrowthSpeedLevel = PlayerPrefs.GetInt($"Farm{i}_GrowthSpeedLevel");
                    farmStat.MultChanceLevel = PlayerPrefs.GetInt($"Farm{i}_MultChanceLevel");

                    farmStatsList.Add(farmStat);
                   
                }
            }
            
            //Inits and Instantiates
            int plEntity = fabric.Value.InstantiatePlayer(isFirstTime, coins);
            
            var playerView = (PlayerView) poolBaseView.Value.Get(plEntity).Value;
            sceneData.Value.Camera.Follow = playerView.transform;
            sceneData.Value.Camera.LookAt = playerView.LookAt;

            for (int i = 0; i < farmStatsList.Count; i++)
            {
                var farmStat = farmStatsList[i];
                var farmEnt = fabric.Value.InitEmptyFarm(sceneData.Value.Farms[i], farmStat);
                if (farmStat.CurrentCulture!=CultureType.NONE)
                {
                    fabric.Value.InstantiateCultureToFarm(farmEnt, staticData.Value.Cultures[farmStat.CurrentCulture]);
                }
               
            }

           
        }
    }
}