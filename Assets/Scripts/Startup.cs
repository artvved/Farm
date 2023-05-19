using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Game.System.Interact;
using Game.Component;
using Game.Service;
using Game.System;
using Game.System.Interact;
using Game.System.Timing;
using LeoEcsPhysics;
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
    private EcsSystems phisSystems;

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;
    void Start()
    {
        world = new EcsWorld();
        var eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        phisSystems = new EcsSystems(world);
        EcsPhysicsEvents.ecsWorld = eventWorld;

        var cultureDataService = new CultureDataService(staticData);

        phisSystems.AddWorld(eventWorld,Idents.EVENT_WORLD)
            
            
            .Add(new MoveApplySystem())
            .Add(new RotationApplySystem())
            .Add(new CollisionSystem())
            
            
            .DelHerePhysics(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
           
            .Inject(sceneData)
            .Inject(staticData)
           // .Inject(new MovementService(world))
           // .Inject(new AnimationService(world))
            .Init();
        
        systems
            .AddWorld(eventWorld,Idents.EVENT_WORLD)
            .Add(new InitWorldSystem())
            
            .Add(new JoystickInputSystem())
            .Add(new ClickInputSystem())
            .Add(new InputMoveSystem())
            
            .Add(new ShotTickSystem())

            .Add(new DefenceSystem())
            .Add(new DamageApplySystem())
            .Add(new HarvestSystem())
            .Add(new LootSpawnSystem())
            .Add(new CultureSpawnTickSystem())
            .Add(new EnemySpawnTickSystem())
            .Add(new LifetimeSystem())
            
            
            .Add(new DestroyDeadSystem())

            .Add(new UpdateCoinsViewSystem())
            
            
            .DelHere<CoinsChangedEventComponent>(Idents.EVENT_WORLD)
            .DelHere<JoystickDragEvent>(Idents.EVENT_WORLD)
            .DelHere<JoystickStartDragEvent>(Idents.EVENT_WORLD)
            .DelHere<JoystickEndDragEvent>(Idents.EVENT_WORLD)
            .DelHere<HarvestEvent>(Idents.EVENT_WORLD)
            .DelHere<SwitchEvent>(Idents.EVENT_WORLD)
            .DelHere<ShotEvent>(Idents.EVENT_WORLD)
            .DelHere<DamageEvent>(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,staticData,sceneData,cultureDataService))
            .Inject(cultureDataService)
            .Inject(sceneData)
            .Inject(staticData)
            .InjectUgui(sceneData.EcsUguiEmitter,Idents.EVENT_WORLD)
            .Init();

    }

    
    void Update()
    {
        systems?.Run();
    }

    private void FixedUpdate()
    {
        phisSystems?.Run();
    }

    private void OnDestroy()
    {
        if (systems!=null)
        {
            systems.Destroy();
            systems = null;
        }
        
        if (phisSystems!=null)
        {
            phisSystems.Destroy();
            phisSystems = null;
        }

        if (world!=null)
        {
            world.Destroy();
            world = null;
        }
        EcsPhysicsEvents.ecsWorld = null;
    }
}
