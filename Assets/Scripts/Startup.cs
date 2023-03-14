using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Game.Component;
using Game.Service;
using Game.System;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using ScriptableData;
using UnityEngine;

public class Startup : MonoBehaviour
{
    private EcsWorld world;
    private EcsSystems systems;
    

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        var eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        
        systems
            .AddWorld(eventWorld,Idents.EVENT_WORLD)
            .Add(new InitPlayerWithCameraSystem())
            
            .Add(new MoveJoystickInputSystem())
            .Add(new MoveSystem())

            .Add(new UpdateCoinsViewSystem())
            
            
            .DelHere<CoinsChangedEventComponent>(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,eventWorld,staticData))
            .Inject(sceneData)
            .Inject(staticData)
            .InjectUgui(sceneData.EcsUguiEmitter,Idents.EVENT_WORLD)
            .Init();
        
       
        
    }

    
    void Update()
    {
        systems?.Run();
    }

    private void OnDestroy()
    {
        if (systems!=null)
        {
            systems.Destroy();
            systems = null;
        }

        if (world!=null)
        {
            world.Destroy();
            world = null;
        }
    }
}
