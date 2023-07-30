using DefaultNamespace;
using Game.Component;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using ScriptableData;

namespace Game.System.Interact
{
    public class LifestealSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;
        private EcsWorld eventWorld;
        private readonly EcsCustomInject<StaticData> data = default;
        
       
        private readonly EcsPoolInject<DamageEvent> createAttackPool = Idents.EVENT_WORLD;
        private readonly EcsPoolInject<Caster> casterPool = default;
        private readonly EcsPoolInject<Health> unitPool = default;
        
        private EcsFilter eventFilter;
        private EcsFilter filterPlayer;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            eventWorld = systems.GetWorld(Idents.EVENT_WORLD);
            eventFilter = eventWorld.Filter<DamageEvent>().Inc<LifestealEvent>().End();
            filterPlayer=world.Filter<PlayerTag>().End();
           
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in eventFilter)
            {
                var attackEventComponent = createAttackPool.Value.Get(entity);

                foreach (var player in filterPlayer)
                {
                    if ( casterPool.Value.Has(player) && attackEventComponent.Target.Unpack(world,out int target))
                    {
                        ref var casterHp = ref unitPool.Value.Get(player).Hp;
                        var maxHp = unitPool.Value.Get(player).MaxHp;
                        var targetMaxHp = unitPool.Value.Get(target).MaxHp;
                        var lifestealMult = data.Value.LifestealPercent / 100;

                        casterHp += (int)(targetMaxHp * lifestealMult);
                        if (casterHp>maxHp)
                        {
                            casterHp = maxHp;
                        }
                   
                    }
                }
                
               
            }
        }
    }
}