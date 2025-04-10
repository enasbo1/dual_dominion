using System.Collections.Generic;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monster
{
    public class SpawnerManager : Manager<SpawnerDealer>
    {
        [SerializeField] private StandByManager<WalkerDdDealer, WalkerEnum> standBy;
        [SerializeField] private WalkerDealingManager walkerDealingManager;

        private readonly List<SpawnerDealer> _spawner = new();
        private readonly List<Transform> _transform = new();
        private readonly List<GameObject> _spawned = new();
        private readonly List<bool> _isActive = new();
        private readonly List<float> _nextSpawnTime = new();

        
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
            _spawned.Add(element.prefabToSpawn);
            _transform.Add(element.transform);
            _nextSpawnTime.Add(Time.time + Random.Range(10,30)/30f);
        }

        public override void DisableElement(SpawnerDealer element)
        {
            int i = _spawner.FindIndex(d => d == element);
            
            if (i != -1)
                _isActive[i] = false;
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < _spawner.Count; i++) if (_isActive[i])
            {
                if (!(_nextSpawnTime[i] < Time.time)) continue;
                if (walkerDealingManager.getNbDealers() > 200) return;
                
                standBy.Spawn(_spawned[i], _transform[i].position, _transform[i].rotation);
                _nextSpawnTime[i] = Time.time + Random.Range(10,30)/30f;
            }
        }
    }
}