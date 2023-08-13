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
        private readonly EcsPoolInject<Coins> poolCoins = default;


        private EcsFilter eventFilter;
        private EcsFilter playerFilter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<CoinsChangedEventComponent>().End();
            playerFilter = world.Filter<PlayerTag>().End();
            
            UpdateUI();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            foreach (var player in playerFilter)
            {
                sceneData.Value.CoinsView.TextMeshProUGUI.text = poolCoins.Value.Get(player).Value.ToString();
            }
        }
    }
}