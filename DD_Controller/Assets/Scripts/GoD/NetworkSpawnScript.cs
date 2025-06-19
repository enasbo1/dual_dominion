using System;
using Monster;
using Shared;
using Unity.Netcode;
using UnityEngine;

namespace GoD
{
    public class MonsterSpawnScript : NetworkSpawnScript<MonsterDealer, WalkerEnum, MonsterVariants>
    {
    }
    public class NetworkSpawnScript<TDealer, TEnum, TVariant> : NetworkBehaviour where TDealer : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        [SerializeField] private StandByManager<TDealer, TEnum, TVariant> standByManager;
        [SerializeField] private DealingManager<TDealer, TEnum, TVariant> dealingManager;
        [SerializeField] private int spawnLimit = 200;
        [SerializeField] private TEnum type;
        
        private float _nextSpawnTime;

        public void SpawnOneRpc(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant variant = default)
        {
            SpawnOneRpc((int)Convert.ChangeType(dealedType, typeof(int)), position, rotation, (int)Convert.ChangeType(variant, typeof(int)));
        }
        
        [Rpc(SendTo.Server)]
        private void SpawnOneRpc(int dealedType, Vector3 position, Quaternion rotation, int variant = default)
        {
            Debug.Log(dealedType);
            spawnOne((TEnum)Enum.ToObject(typeof(TEnum), dealedType), position, rotation, (TVariant)Enum.ToObject(typeof(TEnum),variant));
        }

        private void spawnOne(TEnum dealedType, Vector3 position, Quaternion rotation, TVariant variant = default)
        {
            if (!standByManager.SpawnNetworkObject || NetworkManager.Singleton.IsServer)
            {
                if (dealingManager.GetNbDealers()>spawnLimit) return;

                standByManager.Spawn(dealedType, position, rotation, variant);
                Debug.Log("create Monster");
            }
        }
        private void Update()
        {
            if (NetworkManager.Singleton.IsServer) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnOneRpc(type, transform.position, Quaternion.identity);
            }
        }
    }
}
