using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Leopotam.EcsLite;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;

namespace Game.Service
{
    public class CultureDataService
    {
        private Dictionary<CultureType, CultureData> dictionary;
        

        public CultureDataService(StaticData staticData)
        {
            dictionary = new Dictionary<CultureType, CultureData>();

            var cultures =staticData.Cultures;
            foreach (var cultureData in cultures)
            {
                dictionary.Add(cultureData.CultureType,cultureData);
            }

        }


        public LootView GetLootPrefab(CultureType cultureType)
        {
            return dictionary[cultureType].LootPrefab;
        }
        
        public CultureView GetPrefab(CultureType cultureType)
        {
            return dictionary[cultureType].Prefab;
        }
        
        public CultureData GetData(CultureType cultureType)
        {
            return dictionary[cultureType];
        }




    }
}