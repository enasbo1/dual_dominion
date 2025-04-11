using System;
using System.Diagnostics.CodeAnalysis;
using Monster;
using Shared;
using UnityEngine;

namespace Move
{

    public class WalkerOutOfBoundDieManager : OutOfBoundDieManager<WalkerDdDealer, WalkerEnum, MonsterVariants>
    {
    }
    
    public class OutOfBoundDieManager<TDealer, TEnum, TVariant> : Manager<TDealer, TEnum, TVariant> where TDealer : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        [SerializeField] [NotNull] public StandByManager<TDealer, TEnum, TVariant> standBy;
        public int deathBottom = -100;

        private readonly TableList<Transform> _transform = new(0);

        protected override void AddChunk(int size)
        {
            _transform.AddChunk(size);
        }

        protected override void AddElementInChunk(TDealer element)
        {
            _transform[Size] = element.transform;
        }

        protected override void AddElementInNew(TDealer element)
        {
            _transform.Add(element.transform);
        }
    
    
        // Update is called once per frame
        private void FixedUpdate()
        {
            for (int i = 0 ; i < Size ; i++) if (Active[i] & (_transform[i].position.y < deathBottom))
            {
                standBy.Kill(Elements[i]);
            }
        }
    }
    
}