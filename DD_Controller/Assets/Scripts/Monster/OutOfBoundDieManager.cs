using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Shared;
using UnityEngine;

namespace Monster
{
    public class OutOfBoundDieManager : Manager<WalkerDdDealer>
    {
        [SerializeField] [NotNull] private StandByManager<WalkerDdDealer, WalkerEnum> standBy;
        public int deathBottom = -100;

        private bool[] _active = Array.Empty<bool>();
        private readonly List<Transform> _transform = new();
        private readonly List<WalkerDdDealer> _dealers = new();

        public override void AddElement(WalkerDdDealer element)
        {
            int i = _dealers.FindIndex(d => d == element);

            if (i != -1)
            {
                _active[i] = true;
                return;
            }
            
            _dealers.Add(element);
            _transform.Add(element.transform);
            _active = _active.Append(true).ToArray();
        }

        public override void DisableElement(WalkerDdDealer element)
        {
            int i = _dealers.FindIndex(d => d == element);

            if (i != -1)
                _active[i] = false;
        }
        
        // Update is called once per frame
        private void FixedUpdate()
        {
            for (int i = 0 ; i < _dealers.Count ; i++) if (_active[i] & (_transform[i].position.y < deathBottom))
            {
                standBy.Kill(_dealers[i]);
            }
        }
    }
}