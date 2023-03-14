using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using UnityEngine;

namespace Game.System
{
    public class InitPlayerWithCameraSystem : IEcsInitSystem
    {
        readonly EcsCustomInject<Fabric> fabric=default;
        readonly EcsCustomInject<SceneData> sceneData = default;
        private EcsPool<BaseViewComponent> playerTransformPool;
        

        public void Init(IEcsSystems systems)
        {
            playerTransformPool = systems.GetWorld().GetPool<BaseViewComponent>();
            
            var plEntity=fabric.Value.InstantiatePlayer();
            var playerView = (PlayerView)playerTransformPool.Get(plEntity).Value;
            sceneData.Value.Camera.Follow = playerView.LookAt;
            sceneData.Value.Camera.LookAt = playerView.LookAt;
            
            
        }


       
    }
}