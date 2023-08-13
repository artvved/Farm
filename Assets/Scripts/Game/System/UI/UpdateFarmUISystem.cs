using System;
using DefaultNamespace;
using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;
using Unity.VisualScripting;


namespace Game.System
{
    public class UpdateFarmUISystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;

        private readonly EcsPoolInject<FarmUIUpdateEventComponent> poolEvent = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<CoinsChangedEventComponent> poolMoneyEvent = Idents.EVENT_WORLD;

        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsCustomInject<StaticData> staticData = default;

        private readonly EcsPoolInject<Coins> poolCoins = default;
        private readonly EcsPoolInject<FarmStats> poolFarmStats = default;
        private readonly EcsPoolInject<Culture> poolCulture = default;
        private readonly EcsPoolInject<Tick> poolTick = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<FarmUIUpdateEventComponent>().End();
            playerFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                foreach (var player in playerFilter)
                {
                    ref var coins = ref poolCoins.Value.Get(player).Value;

                    var farmEntity = poolEvent.Value.Get(entity).FarmEntity;
                    ref var farmStats = ref poolFarmStats.Value.Get(farmEntity);
                    var data = staticData.Value;
                    var screen = sceneData.Value.FarmUIScreen;

                    if (farmStats.CurrentCulture==CultureType.NONE)
                    {
                        screen.UpdateEmpty();
                        screen.Open();
                        continue;
                    }

                    var cultureData = data.Cultures[farmStats.CurrentCulture];
                    var gsProg = data.GrowthSpeedKProgression;
                    var gscProg = data.GrowthSpeedKCostProgression;
                    var gsLvl = farmStats.GrowthSpeedLevel;
                    
                    screen.UpdateLabel(cultureData.Icon,  cultureData.Culture.CultureType);
                    

                    SetupButton(
                        farmEntity,
                        gsProg,
                        gscProg,
                        gsLvl,coins,
                        screen.upTimeButton,
                        UpdateGsLevel);
                    SetupButton(farmEntity,
                        data.MultKProgression,
                        data.MultKCostProgression,
                        farmStats.MultChanceLevel,
                        coins,
                        screen.upChanceButton,
                        UpdateMultLevel);
                   
                    screen.Open();
                }
            }
        }

        private void GiveMoney(int cost)
        {
            foreach (var player in playerFilter)
            {
                ref var coins = ref poolCoins.Value.Get(player);
                coins.Value -= cost;
                poolMoneyEvent.NewEntity(out int entity);
            }
        }

        private void UpdateGsLevel(int farmEnt,float k,int cost)
        {
            ref var farmStats = ref poolFarmStats.Value.Get(farmEnt);
            farmStats.GrowthSpeedLevel++;
            foreach (var cultEnt in farmStats.CultureEntities)
            {
                poolCulture.Value.Get(cultEnt).GrowthTime = k;
                poolTick.Value.Get(cultEnt).FinalTime = k;
            }

            GiveMoney(cost);
            poolEvent.NewEntity(out int ev).FarmEntity=farmEnt;
        }
        
        private void UpdateMultLevel(int farmEnt,float k,int cost)
        {
            ref var farmStats = ref poolFarmStats.Value.Get(farmEnt);
            farmStats.MultChanceLevel++;
            foreach (var cultEnt in farmStats.CultureEntities)
            {
                poolCulture.Value.Get(cultEnt).MultChance *= k;
            }

            GiveMoney(cost);
            poolEvent.NewEntity(out int ev).FarmEntity=farmEnt;
        }

        private void SetupButton(int farmEnt,float[] prog, int[] costProg, int level,int coins, UpButton upButton,Action<int,float,int> clickAction)
        {
            if (IsMax(prog, level))
                upButton.SetupButtonMax();
            else
            {
                var from = prog[level];
                var to = prog[level+1];
                var cost = costProg[level+1];
                        
                upButton.SetupButton(from, to, cost, coins >= cost);
                upButton.button.onClick.RemoveAllListeners();
                upButton.button.onClick.AddListener(()=>clickAction?.Invoke(farmEnt,to,cost));
            }
        }

        private bool IsMax(float[] progression, int level)
        {
            return level +1>= progression.Length;
        }
        
    }
}