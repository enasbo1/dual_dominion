using System;
using JetBrains.Annotations;
using Monster;
using Move;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Shared
{
    public class WalkerDdDealer : WalkerDealer<WalkerEnum, MonsterVariants>
    {
    }
    
    public class WalkerDealer<TEnum, TVariant> : Dealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        [Header("Walker Body References")]
        [SerializeField] [CanBeNull] public Rigidbody body;
        [SerializeField] [CanBeNull] public Transform headTransform;
        [SerializeField] [CanBeNull] public MoveScript moveScript;
        [SerializeField] [CanBeNull] public Animator animator;
        [SerializeField] [CanBeNull] public Renderer[] witnessBlessing;
        
        [Header("Walker Characteristics")]
        [SerializeField] public float size = 1;
        [SerializeField] public float maxHealth = 1f;
        [DoNotSerialize] public int group;

        public override void ResetDealed(bool respawn)
        {
            if (respawn)
            {
                Rigidbody rb = mainTransform?.GetComponent<Rigidbody>();
                if (rb)
                    rb.ResetInertiaTensor();
            }

            if (moveScript) moveScript.couldMove = false;
        }
    }

    public class Dealer<TEnum, TVariant> : Dealer where TEnum : Enum where TVariant : Enum
    {
        [Header("Type References")]

        [SerializeField] public TEnum type;
        
        public virtual void ApplyVariant(TVariant variant)
        {
        }
    }

    public class Dealer : MonoBehaviour
    {
        [Header("Dealer References")]
        [SerializeField] [CanBeNull] public Transform mainTransform;
        [SerializeField] [CanBeNull] public NetworkObject networkObject;

        public virtual void ResetDealed(bool respawn)
        {
        }

        public virtual void Kill()
        {
            gameObject.SetActive(false);
        }
    }
}