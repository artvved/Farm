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
    public class MoveJoystickInputSystem : EcsUguiCallbackSystem,IEcsInitSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsCustomInject<SceneData> sceneData = default;

        private readonly EcsPoolInject<PlayerStatsComponent> playerStatsPool = default;
        private readonly EcsPoolInject<SpeedComponent> speedPool = default;
        private readonly EcsPoolInject<DirectionComponent> directionPool = default;
        
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
           
            playerFilter = world.Filter<PlayerTag>().Exc<CantMoveComponent>().End();
        }

        [Preserve]
        [EcsUguiDragMoveEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDrag (in EcsUguiDragMoveEvent e) {
            foreach (var ent in playerFilter)
            {
                var maxSpeed = playerStatsPool.Value.Get(ent).MaxSpeed;
                var joystickDirection = sceneData.Value.Joystick.Direction;
                speedPool.Value.Get(ent).Value =  maxSpeed;
                directionPool.Value.Get(ent).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
            }
        }
        
        [Preserve]
        [EcsUguiDragStartEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragStart (in EcsUguiDragStartEvent e) {
            foreach (var ent in playerFilter)
            {
                var maxSpeed = playerStatsPool.Value.Get(ent).MaxSpeed;
                var joystickDirection = sceneData.Value.Joystick.Direction;
                speedPool.Value.Add(ent).Value = joystickDirection.magnitude * maxSpeed;
                directionPool.Value.Add(ent).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
            }
        }

        [Preserve]
        [EcsUguiDragEndEvent(Idents.Ui.MoveJoystick, Idents.EVENT_WORLD)]
        void OnDragEnd (in EcsUguiDragEndEvent e) {
            foreach (var ent in playerFilter)
            {
                speedPool.Value.Del(ent);
                directionPool.Value.Del(ent);
            }
        }

    }
}