using DefaultNamespace;
using Game.Component;

using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;



namespace Game.System
{
    public class UpdateCoinsViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<SceneData> sceneData = default;
        private readonly EcsPoolInject<PlayerStatsComponent> unitPool = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<CoinsChangedEventComponent>().End();
            playerFilter = world.Filter<PlayerTag>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                foreach (var player in playerFilter)
                {
                    sceneData.Value.CoinsView.TextMeshProUGUI.text = unitPool.Value.Get(player).Coins.ToString();
                }
            }
        }
    }
}