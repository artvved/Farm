using Cinemachine;
using Game.Mono;
using Game.Service;
using Game.UI;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;

namespace ScriptableData
{
    public class SceneData : MonoBehaviour
    {
        public EcsUguiEmitter EcsUguiEmitter;
        public CinemachineVirtualCamera Camera;
        public CoinsView CoinsView;
        public FloatingJoystick Joystick;

        public Transform EnemySpawnPlace;
        
        
        public FarmView[] Farms;

    }
}