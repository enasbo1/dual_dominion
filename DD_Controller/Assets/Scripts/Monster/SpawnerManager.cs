using System;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monster
{
    public class SpawnerManager : Manager<SpawnerDealer, Enum>
    {
        [SerializeField] private WalkerStandByManager standBy;
        [SerializeField] private WalkerDealingManager walkerDealingManager;

        private readonly TableList<Transform> _transform = new(0);
        private readonly TableList<GameObject> _spawned = new(0);
        private readonly TableList<float> _nextSpawnTime = new(0);


        protected override void _InitializeChunk(int size)
        {
            _spawned.AddChunk(size);
            _transform.AddChunk(size);
            _nextSpawnTime.AddChunk(size);
        }

        protected override void AddElementInChunk(SpawnerDealer element)
        {
            _spawned[Size] = element.prefabToSpawn;
            _transform[Size] = element.transform;
            _nextSpawnTime[Size] = Time.time + Random.Range(10, 30) / 30f;
        }

        protected override void AddElementInNew(SpawnerDealer element)
        {
            _spawned.Add(element.prefabToSpawn);
            _transform.Add(element.transform);
            _nextSpawnTime.Add(Time.time + Random.Range(10, 30) / 30f);
        }

        private void FixedUpdate()
        {
            for (int i = 0; i < Size; ++i) if (Active[i])
            {
                if (!(_nextSpawnTime[i] < Time.time)) continue;
                if (walkerDealingManager.getNbDealers() > 200) return;

                standBy.Spawn(_spawned[i], _transform[i].position, _transform[i].rotation);
                _nextSpawnTime[i] = Time.time + Random.Range(10, 30) / 30f;
            }
        }
    }
}