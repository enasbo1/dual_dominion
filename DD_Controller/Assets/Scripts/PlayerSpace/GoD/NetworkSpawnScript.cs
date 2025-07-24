using System;
using Monster;
using PlayerSpace.GoD;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace GoD
{
    public class MonsterSpawnScript : NetworkSpawnScript<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        public GodManagerScript godManager;
        public GodSelectMonsterManagerScript godSelectMonsterManagerScript;

        private NetworkManager _networkManager;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
        }

        private void Update()
        {
            if (!spawnLocation?.gameObject.activeSelf?? true) return; 
            if (_networkManager.IsServer) return;
            if (Input.GetKeyDown(KeyCode.Space) && type != WalkerEnum.None)
            {
                PlayerSpace.GoD.Monster monsterSpawn = godSelectMonsterManagerScript.GetMonsterSpawnerByType(type, variant);
                if (monsterSpawn == null) return;
                if (godManager.karmaPoint > monsterSpawn.cost)
                {
                    godManager.karmaPoint -= monsterSpawn.cost;
                    SpawnOneRpc(type, godSelectMonsterManagerScript.monsterSpawnPoint.position, godSelectMonsterManagerScript.monsterSpawnPoint.rotation, variant);
                }
            }
        }
        
    }
    public class NetworkSpawnScript<TDealer, TEnum, TVariant> : NetworkBehaviour where TDealer : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        [SerializeField] public StandByManager<TDealer, TEnum, TVariant> standByManager;
        [SerializeField] private DealingManager<TDealer, TEnum, TVariant> dealingManager;
        [SerializeField] public Transform spawnLocation;
        [SerializeField] private int spawnLimit = 200;
        public TEnum type;
        public TVariant variant;
        
        private float _nextSpawnTime;

        public void SpawnOneRpc(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant dealedVariant = default)
        {
            SpawnOneRpc((int)Convert.ChangeType(dealedType, typeof(int)), position, rotation, (int)Convert.ChangeType(dealedVariant, typeof(int)));
        }
        
        [Rpc(SendTo.Server)]
        private void SpawnOneRpc(int dealedType, Vector3 position, Quaternion rotation, int dealedVariant = default)
        {
            spawnOne((TEnum)Enum.ToObject(typeof(TEnum), dealedType), position, rotation, (TVariant)Enum.ToObject(typeof(TEnum),dealedVariant));
        }

        private void spawnOne(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant dealedVariant = default)
        {
            if (!standByManager.SpawnNetworkObject || NetworkManager.Singleton.IsServer)
            {
                if (dealingManager.GetNbDealers()>spawnLimit) return;

                standByManager.Spawn(dealedType, position, rotation, dealedVariant);
            }
        }
    }
}
