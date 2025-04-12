using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class MonsterDealingManager : DealingManager<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        [SerializeField] [CanBeNull]
        public DealingManager<WalkerDdDealer, WalkerEnum, MonsterVariants> ParentDealingManager;

        private void Start()
        {
            ContextStart(_AddElement, _RemoveElement);
        }


        private void _AddElement(MonsterDealer element)
        {
            AddElement(element);
            ParentDealingManager?.Add(element);
        }

        private void _RemoveElement(MonsterDealer element)
        {
            RemoveElement(element);
            ParentDealingManager?.Remove(element);
        }
    }
}