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
        [Header("UI")]
        public CoinsView CoinsView;
        public FloatingJoystick Joystick;
        
        public FarmUIScreen FarmUIScreen;
        
        [Header("Level")]
        public Transform EnemySpawnPlace;
        public FarmView[] Farms;

        public bool StartNewData;

    }
}