using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Shared
{
    public class WalkerStandByManager : StandByManager<WalkerDdDealer, WalkerEnum>
    {
    }

    public abstract class StandByManager<TDealer, TEnum> : MonoBehaviour where  TDealer : Dealer<TEnum> where TEnum : Enum
    {
        [SerializeField] [CanBeNull] private DealingManager<TDealer> dealingManager;
        
        private readonly List<TDealer> _dealers = new ();
        private readonly List<bool> _isDead = new ();

        public TDealer Spawn(GameObject objectToSpawn, Vector3 position, Quaternion rotation, bool deal = true)
        {
            int i = -1;
            TEnum type = objectToSpawn.GetComponent<TDealer>().type;
            for (int j = 0; j < _dealers.Count; j++) if (_isDead[j])
            {
                if (type == null) break;
                if (!type.Equals(_dealers[j].type)) continue;
                i = j;
                break;
            }

            TDealer newDealer;
            if (i == -1)
            {
                newDealer = Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity).GetComponent<TDealer>();
                newDealer.Reset(false);
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


            
            if (!newDealer.mainTransform) return newDealer;

            newDealer.mainTransform.position = position;
            newDealer.mainTransform.rotation = rotation;


            
            
            if (deal && dealingManager)
                dealingManager.Add(newDealer);
            
            return newDealer;
        }

        public TDealer Kill(TDealer dealer, bool deal = true)
        {
            dealer.gameObject.SetActive(false);
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