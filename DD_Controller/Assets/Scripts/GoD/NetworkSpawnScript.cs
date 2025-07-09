using System;
using Monster;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace GoD
{
    public class MonsterSpawnScript : NetworkSpawnScript<MonsterDealer, WalkerEnum, MonsterVariants>
    {
        public GodManagerScript godManagerScript;
        
        private void Update()
        {
            if (NetworkManager.Singleton.IsServer) return;
            if (Input.GetKeyDown(KeyCode.Space) && type != WalkerEnum.None)
            {
                MonsterSpawnButton monsterSpawn = godManagerScript.GetMonsterSpawnerByType(type);

                if (godManagerScript.karmaPoint > monsterSpawn.cost)
                {
                    godManagerScript.karmaPoint -= monsterSpawn.cost;
                    SpawnOneRpc(type, spawnLocation.position, Quaternion.identity);
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
        
        private float _nextSpawnTime;

        public void SpawnOneRpc(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant variant = default)
        {
            SpawnOneRpc((int)Convert.ChangeType(dealedType, typeof(int)), position, rotation, (int)Convert.ChangeType(variant, typeof(int)));
        }
        
        [Rpc(SendTo.Server)]
        private void SpawnOneRpc(int dealedType, Vector3 position, Quaternion rotation, int variant = default)
        {
            spawnOne((TEnum)Enum.ToObject(typeof(TEnum), dealedType), position, rotation, (TVariant)Enum.ToObject(typeof(TEnum),variant));
        }

        private void spawnOne(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant variant = default)
        {
            if (!standByManager.SpawnNetworkObject || NetworkManager.Singleton.IsServer)
            {
                if (dealingManager.GetNbDealers()>spawnLimit) return;

                standByManager.Spawn(dealedType, position, rotation, variant);
            }
        }
    }
}
