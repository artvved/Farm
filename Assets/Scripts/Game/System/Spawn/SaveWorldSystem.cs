using System.Collections.Generic;
using System.Linq;
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
    public class SaveWorldSystem : IEcsInitSystem,IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<Fabric> fabric=default;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private EcsPoolInject<Coins> poolMoney; 
        private EcsPoolInject<PlayerStats> poolPlayer; 
        private EcsPoolInject<FarmStats> poolFarm; 


        private EcsFilter filterEvent;
        private EcsFilter filterPlayer;
        private EcsFilter filterFarm; 

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            filterEvent= eventWorld.Filter<SaveGameEvent>().End();
            filterPlayer = world.Filter<PlayerStats>().End();
            filterFarm = world.Filter<FarmStats>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var ev in filterEvent)
            {
                Save();
            }
            
        }


        public  void Save()
        {
            foreach (var playerE in filterPlayer)
            {
                PlayerPrefs.SetInt("Money", poolMoney.Value.Get(playerE).Value);
                PlayerPrefs.SetInt("IsFirstTime",1/*poolPlayer.Value.Get(playerE).IsFirstTime?0:1*/);
            }

            int i = 0;
            foreach (var farmE in filterFarm)
            {
                var farmStat = poolFarm.Value.Get(farmE);
                PlayerPrefs.SetInt($"Farm{i}_CultureType",(int)farmStat.CurrentCulture );
                PlayerPrefs.SetInt($"Farm{i}_GrowthSpeedLevel",farmStat.GrowthSpeedLevel );
                PlayerPrefs.SetInt($"Farm{i}_MultChanceLevel",farmStat.MultChanceLevel );
                if (farmStat.GroundPicture==null)
                {
                    PlayerPrefsExtra.SetList($"Farm{i}_GroundPicture",new List<Color>());
                }
                else
                {  
                    
                    Color[] png =farmStat.GroundPicture.GetPixels();
                    PlayerPrefsExtra.SetList($"Farm{i}_GroundPicture",png.ToList());
                }
                i++;
            }

           // PlayerPrefs.SetInt("FarmsCount",i);

        }

       
    }
}