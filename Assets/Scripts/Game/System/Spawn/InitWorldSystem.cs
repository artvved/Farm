using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class InitWorldSystem : IEcsInitSystem
    {
        private readonly EcsCustomInject<Fabric> fabric=default;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;
    

        public void Init(IEcsSystems systems)
        {
            var plEntity=fabric.Value.InstantiatePlayer();

            var farms = sceneData.Value.Farms;
            foreach (var farm in farms)
            {
                fabric.Value.InitFarm(farm,staticData.Value.Cultures[0]);
            }
        }


       
    }
}