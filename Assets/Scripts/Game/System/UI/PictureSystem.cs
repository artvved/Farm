using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Mono;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;


namespace Game.System
{
    public class PictureSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<FarmUIUpdateEventComponent> poolEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<CoinsChangedEventComponent> poolMoneyEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<PictureLoadedEvent> poolPicLoaded = Idents.EVENT_WORLD;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<Coins> poolCoins = default;
        private readonly EcsPoolInject<FarmStats> poolFarmStats = default;
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<BaseViewComponent> poolView = default;
        private readonly EcsPoolInject<Tick> poolTick = default;
        
        private readonly EcsPoolInject<PictureLoadedEvent> poolPic = Idents.EVENT_WORLD;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<PictureLoadedEvent>().End();
            playerFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var e in eventFilter)
            {
                var pictureLoadedEvent = poolPic.Value.Get(e);
                //calculate values
                ref var farmStats =ref poolFarmStats.Value.Get(pictureLoadedEvent.Value.Farm);
                farmStats.GroundPicture = pictureLoadedEvent.Value.Picture;

                //set view
                var farmView = (FarmView) poolView.Value.Get(pictureLoadedEvent.Value.Farm).Value;
                farmView.PictureGround.gameObject.SetActive(true);
                farmView.PictureGround.material.mainTexture = pictureLoadedEvent.Value.Picture;
                farmView.DefaultGround.SetActive(false);

            }
        }

      
      
      

    }
}