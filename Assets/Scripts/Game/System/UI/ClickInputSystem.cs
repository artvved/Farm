using DefaultNamespace;
using Game.Component;

using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;
using UnityEngine.Scripting;


namespace Game.System
{
    public class ClickInputSystem : EcsUguiCallbackSystem,IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<SwitchEvent> poolSwitch = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<ShotEvent> poolShot = Idents.EVENT_WORLD;
        
      

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
        }

        [Preserve]
        [EcsUguiClickEvent(Idents.Ui.Switch, Idents.EVENT_WORLD)]
        void OnSwitch (in EcsUguiClickEvent e) {
            poolSwitch.NewEntity(out int entity);
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.Ui.Shot, Idents.EVENT_WORLD)]
        void OnShot (in EcsUguiClickEvent e) {
            poolShot.NewEntity(out int entity);
        }
        
       

    }
}