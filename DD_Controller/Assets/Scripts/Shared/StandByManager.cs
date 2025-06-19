using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Monster;
using UnityEngine;

namespace Shared
{
    public class WalkerStandByManager : StandByManager<WalkerDdDealer, WalkerEnum, MonsterVariants>
    {
    }

    public abstract class StandByManager<TDealer, TEnum, TVariant> : MonoBehaviour
        where TDealer : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        [SerializeField] [CanBeNull] private DealingManager<TDealer, TEnum, TVariant> dealingManager;
        [SerializeField] [CanBeNull] private PrefabReferencer<TDealer, TEnum, TVariant> prefabReferencer;
        [SerializeField] private bool spawnNetworkObject;
        
        public bool SpawnNetworkObject => spawnNetworkObject;
        private readonly List<TDealer> _dealers = new();
        private readonly List<bool> _isDead = new();

        public TDealer Spawn(TEnum typeKey, Vector3 position, Quaternion rotation, TVariant variant = default,
            bool deal = true)
        {
            if (!prefabReferencer)
                throw new Exception("No PrefabReferencer Set, Can't Spawn");
            return Spawn(prefabReferencer[typeKey].Item1, position, rotation, variant, deal);
        }

        public TDealer Spawn(GameObject objectToSpawn, Vector3 position, Quaternion rotation,
            TVariant variant = default, bool deal = true)
        {
            int i = -1;
            TEnum type = objectToSpawn.GetComponent<TDealer>().type;
            for (int j = 0; j < _dealers.Count; j++)
                if (_isDead[j])
                {
                    if (type == null) break;
                    if (!type.Equals(_dealers[j].type)) continue;
                    if (_dealers[j].isActiveAndEnabled) continue;
                    i = j;
                    break;
                }

            TDealer newDealer;
            if (i == -1)
            {
                newDealer = Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity).GetComponent<TDealer>();
                newDealer.ResetDealed(false);
                if (newDealer.networkObject)
                    if (spawnNetworkObject)
                        newDealer.networkObject.Spawn();
                    else
                        newDealer.networkObject.enabled = false;
            }
            else
            {
                newDealer = _dealers[i];
                _isDead[i] = false;
                Rigidbody rb = newDealer.mainTransform?.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.ResetInertiaTensor();
                    rb.linearVelocity = Vector3.zero;
                }
            }

            newDealer.gameObject.SetActive(true);
            newDealer.ApplyVariant(variant);
            
            if (!newDealer.mainTransform) return newDealer;

            newDealer.mainTransform.position = position;
            newDealer.mainTransform.rotation = rotation;


            if (deal && dealingManager)
                dealingManager.Add(newDealer);

            return newDealer;
        }

        public TDealer Kill(TDealer dealer, bool deal = true)
        {
            dealer.Kill();
            int i = _dealers.FindIndex(d => d == dealer);


            if (i == -1)
            {
                _dealers.Add(dealer);
                _isDead.Add(true);
            }
            else
            {
                _isDead[i] = true;
            }

            if (deal && dealingManager)
                dealingManager.Remove(dealer);

            return dealer;
        }
    }
}