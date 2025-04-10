using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class OutOfBoundDieManager : WalkerManager
    {
        [SerializeField] [NotNull] private StandByManager<WalkerDdDealer, WalkerEnum> standBy;
        public int deathBottom = -100;

        private readonly TableList<Transform> _transform = new(0);

        protected override void _InitializeChunk(int size)
        {
            _transform.AddChunk(size);
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _transform[Size] = element.transform;
        }

        protected override void AddElementInNew(WalkerDdDealer element)
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