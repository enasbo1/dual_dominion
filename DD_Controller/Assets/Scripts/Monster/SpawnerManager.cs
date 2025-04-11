using System;
using Shared;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Monster
{
    public class SpawnerManager : Manager<SpawnerDealer, Enum, Enum>
    {
        [SerializeField] private MonsterStandByManager standBy;
        [SerializeField] private MonsterDealingManager monsterDealingManager;
        [SerializeField] private MonsterReferencer monsterReferencer;

        private readonly TableList<Transform> _transform = new(0);
        private readonly TableList<WalkerEnum[]> _spawned = new(0);
        private readonly TableList<float> _nextSpawnTime = new(0);


        protected override void AddChunk(int size)
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
                if (monsterDealingManager.GetNbDealers() > 200) return;
                
                WalkerEnum spawn = _spawned[i][Random.Range(0, _spawned[i].Length)];
                (GameObject prefab, MonsterDealer dealer) = monsterReferencer[spawn];
                if (dealer.variants.Length > 0)
                    standBy.Spawn(prefab, _transform[i].position, _transform[i].rotation, dealer.variants[Random.Range(0, dealer.variants.Length)]);
                else standBy.Spawn(prefab, _transform[i].position, _transform[i].rotation);
                _nextSpawnTime[i] = Time.time + Random.Range(10, 30) / 30f;
            }
        }
    }
}