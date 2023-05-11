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
    public class JoystickInputSystem : EcsUguiCallbackSystem,IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        
        private readonly EcsPoolInject<JoystickStartDragEvent> poolStartDrag = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<JoystickEndDragEvent> poolEndDrag = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<JoystickDragEvent> poolDrag = Idents.EVENT_WORLD;
        
      

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
        }

        [Preserve]
        [EcsUguiDragMoveEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDrag (in EcsUguiDragMoveEvent e) {
            poolDrag.NewEntity(out int entity).Value=sceneData.Value.Joystick.Direction;
        }
        
        [Preserve]
        [EcsUguiDragStartEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragStart (in EcsUguiDragStartEvent e)
        {
            poolStartDrag.NewEntity(out int entity).Value=sceneData.Value.Joystick.Direction;
        }

        [Preserve]
        [EcsUguiDragEndEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragEnd (in EcsUguiDragEndEvent e) {
            poolEndDrag.NewEntity(out int entity);
        }

    }
}