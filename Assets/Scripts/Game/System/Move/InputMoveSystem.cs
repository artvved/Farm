using DefaultNamespace;
using Game.Component;
using Game.Mono;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Mitfart.LeoECSLite.UnityIntegration;
using ScriptableData;
using UnityEngine;


namespace Game.System
{
    public class InputMoveSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        
        private readonly EcsPoolInject<PlayerStats> playerStatsPool = default;
        private readonly EcsPoolInject<Speed> speedPool = default;
        private readonly EcsPoolInject<Direction> directionPool = default;
        
        private readonly EcsPoolInject<JoystickDragEvent> poolDrag = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<JoystickStartDragEvent> poolStartDrag = Idents.EVENT_WORLD;

        private EcsFilter filterPlayer;
        private EcsFilter filterStartDrag;
        private EcsFilter filterEndDrag;
        private EcsFilter filterDrag;


        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            filterStartDrag = eventWorld.Filter<JoystickStartDragEvent>().End();
            filterEndDrag = eventWorld.Filter<JoystickEndDragEvent>().End();
            filterDrag = eventWorld.Filter<JoystickDragEvent>().End();
            filterPlayer = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var player in filterPlayer)
            {
                foreach (var eventEntity in filterStartDrag)
                {
                    var maxSpeed = GetPlayerMaxSpeed(player);
                    var joystickDirection = poolStartDrag.Value.Get(eventEntity).Value;
                    speedPool.Value.Add(player).Value = joystickDirection.magnitude * maxSpeed;
                    directionPool.Value.Get(player).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
                }
                
                foreach (var eventEntity in filterDrag)
                {
                    var maxSpeed = GetPlayerMaxSpeed(player);
                    var joystickDirection = poolDrag.Value.Get(eventEntity).Value;
                    speedPool.Value.Get(player).Value = joystickDirection.magnitude * maxSpeed;
                    directionPool.Value.Get(player).Value = new Vector3(joystickDirection.x, 0, joystickDirection.y);
                }
                
                foreach (var eventEntity in filterEndDrag)
                {
                    speedPool.Value.Del(player);
                    //directionPool.Value.Del(player);
                }
                
                
            }
        }

        private float GetPlayerMaxSpeed(int player)
        {
            return playerStatsPool.Value.Get(player).MaxSpeed;
        }
    }
}