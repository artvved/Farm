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
    private EcsWorld eventWorld;
    private EcsSystems systems;
    private EcsSystems phisSystems;

    [SerializeField]
    private SceneData sceneData;
    [SerializeField]
    private StaticData staticData;

    private SaveWorldSystem saveWorldSystem;

    void Start()
    {
        Input.multiTouchEnabled = false;
        
        world = new EcsWorld();
        eventWorld = new EcsWorld();
        systems = new EcsSystems(world);
        phisSystems = new EcsSystems(world);
        EcsPhysicsEvents.ecsWorld = eventWorld;
        saveWorldSystem = new SaveWorldSystem();
        

        phisSystems.AddWorld(eventWorld,Idents.EVENT_WORLD)
            .Add(new MoveApplySystem())
            .Add(new RotationApplySystem())
            .Add(new CollisionSystem())
            
            .Add(new DestroyDeadSystem())
            
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
            .Add(new LoadWorldSystem())
            
            .Add(new JoystickInputSystem())
            .Add(new ClickInputSystem())
            .Add(new InputMoveSystem())
            
            .Add(new ShotTickSystem())

            .Add(new DefenceSystem())
            .Add(new DamageApplySystem())
            .Add(new HarvestSystem())
            .Add(new LootSpawnSystem())
            .Add(new LootInventorySystem())
            .Add(new CultureSpawnTickSystem())
            .Add(new EnemySpawnTickSystem())
            .Add(new LifetimeSystem())
            
            
            .Add(new UpdateFarmUISystem())
            .Add(new UpdateCoinsViewSystem())
            
           
            .Add(new DestroyDeadSystem())
            
            .Add(saveWorldSystem)
            
            
            .DelHere<CoinsChangedEventComponent>(Idents.EVENT_WORLD)
            .DelHere<FarmUIUpdateEventComponent>(Idents.EVENT_WORLD)
            .DelHere<JoystickDragEvent>(Idents.EVENT_WORLD)
            .DelHere<JoystickStartDragEvent>(Idents.EVENT_WORLD)
            .DelHere<JoystickEndDragEvent>(Idents.EVENT_WORLD)
            .DelHere<HarvestEvent>(Idents.EVENT_WORLD)
            .DelHere<SwitchEvent>(Idents.EVENT_WORLD)
            .DelHere<ShotEvent>(Idents.EVENT_WORLD)
            .DelHere<DamageEvent>(Idents.EVENT_WORLD)
            .DelHere<LootInventoryEvent>(Idents.EVENT_WORLD)
            .DelHere<SaveGameEvent>(Idents.EVENT_WORLD)
          
#if UNITY_EDITOR
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem ())
            .Add (new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem (Idents.EVENT_WORLD))
#endif
            .Inject(new Fabric(world,staticData,sceneData))
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
#if UNITY_EDITOR
        saveWorldSystem.Save();
#else
        eventWorld.GetPool<SaveGameEvent>().Add(eventWorld.NewEntity());
#endif

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
