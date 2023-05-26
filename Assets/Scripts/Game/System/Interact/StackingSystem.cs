using DefaultNamespace.Game.Component.Time;
using Game.Component;
using Game.Service;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Game.System.Interact
{
    public class StackingSystem: IEcsInitSystem, IEcsRunSystem
    {
        private EcsWorld world;

        private EcsPoolInject<Tick> tickPool = default;
        private EcsPoolInject<StackComponent> stackPool = default;
        private EcsCustomInject<MovementService> service = default;
        private EcsPoolInject<ItemTranslationComponent> putPool = default;
     
        private EcsPoolInject<StackFinishedComponent> finishedPool = default;

        private EcsFilter filter;

        public void Init(IEcsSystems systems)
        {
            world = systems.GetWorld();
            filter = world.Filter<ItemTranslationComponent>().Inc<Tick>().End();
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var unit in filter)
            {
                int inUnit = unit;
                var itemTranslationComponent = putPool.Value.Get(inUnit);
                itemTranslationComponent.Target.Unpack(world, out int target);

                ref var tickComponent = ref tickPool.Value.Get(inUnit);

                var isPutting = itemTranslationComponent.IsPutting;
                if (!isPutting)
                {
                    (target, inUnit) = (inUnit, target);
                }

                //no place to put
                ref var targetStackComponent = ref stackPool.Value.Get(target);
                if (IsTargetFull(targetStackComponent))
                    continue;
                //no items to put
                ref var giverStackComponent = ref stackPool.Value.Get(inUnit);
                if (IsGiverEmpty(giverStackComponent))
                    continue;


                if (tickComponent.CurrentTime >= tickComponent.FinalTime)
                {
                    giverStackComponent.Entities[giverStackComponent.CurrCapacity - 1].Unpack(world, out int item);

                    service.Value.TranslateItem(item, inUnit,target);
                    
                    if (IsGiverEmpty(giverStackComponent) && isPutting)
                    {
                        finishedPool.Value.Add(inUnit);
                    }
                    else if (IsTargetFull(targetStackComponent) && !isPutting)
                    {
                        finishedPool.Value.Add(target).IsTaker=true;
                    }


                    tickComponent.CurrentTime = 0;
                }

                tickComponent.CurrentTime += Time.deltaTime;
            }
        }

        private bool IsTargetFull(StackComponent component)
        {
            return component.CurrCapacity >= component.MaxCapacity;
        }

        private bool IsGiverEmpty(StackComponent component)
        {
            return component.CurrCapacity == 0;
        }
        

      
    }
}