using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared
{
    public class WalkerReferencer : PrefabReferencer<WalkerDdDealer, WalkerEnum, Monster.MonsterVariants>{}
    
    public class PrefabReferencer<TDealer, TEnum, TVariant> : MonoBehaviour where TDealer : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        public TDealer[] prefabDealer;

        public readonly Dictionary<TEnum, (GameObject, TDealer)> PrefabDealerDict = new();
        
        public (GameObject, TDealer) this[TEnum index] => PrefabDealerDict[index];

        private void Start()
        {
            foreach (TDealer dealer in prefabDealer)
            {
                if (PrefabDealerDict.ContainsKey(dealer.type))
                
                    Debug.LogWarning($"TypeKey doubloon : {dealer.type} ");
                else
                    PrefabDealerDict.Add(dealer.type, (dealer.gameObject, dealer));
            }
        }
    }
}