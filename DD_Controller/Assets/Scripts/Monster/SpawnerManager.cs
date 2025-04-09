using System.Collections.Generic;
using Shared;
using UnityEngine;

namespace Monster
{
    public class SpawnerManager : Manager<SpawnerDealer>
    {
        [SerializeField] private WalkerDealingManager prefabToSpawn;
        
        private readonly List<SpawnerDealer> _spawner = new();
        private readonly List<Transform> _transform = new();
        private readonly List<GameObject> _spawned = new();
        private readonly List<bool> _isActive = new();

        
        public override void AddElement(SpawnerDealer element)
        {
            int i = _spawner.FindIndex(d => d == element);

            if (i != -1)
            {
                _isActive[i] = true;
                return;
            }
            
            _spawner.Add(element);
            _isActive.Add(true);
            _spawned.Add(element.gameObject);
            _transform.Add(element.transform);
        }

        public override void DisableElement(SpawnerDealer element)
        {
            int i = _spawner.FindIndex(d => d == element);
            
            if (i != -1)
                _isActive[i] = false;
        }
    }
}