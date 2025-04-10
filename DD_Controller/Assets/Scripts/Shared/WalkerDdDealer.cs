using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Move;
using UnityEngine;

namespace Shared
{
    public class WalkerDdDealer : Dealer<WalkerEnum>
    {
        [SerializeField] [CanBeNull] public Rigidbody body;
        [SerializeField] [CanBeNull] public Transform headTransform;
        [SerializeField] [CanBeNull] public MoveScript moveScript;
        [SerializeField] [CanBeNull] public Animator animator;
        [SerializeField] [CanBeNull] public List<Renderer> witnessBlessing;


        public new void Reset(bool respawn)
        {
            if (respawn)
            {
                Rigidbody rb = mainTransform?.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.ResetInertiaTensor();
                }
            }
            
            if (moveScript) moveScript.couldMove = false;
        }
    }

    public class Dealer<TEnum> : MonoBehaviour where TEnum : Enum
    {
        [SerializeField] [CanBeNull] public TEnum type;
        
        [SerializeField] [CanBeNull] public Transform mainTransform;

        public void Reset(bool respawn) {}
    }
}