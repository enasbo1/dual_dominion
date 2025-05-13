using System;
using JetBrains.Annotations;
using Monster;
using Move;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace Shared
{
    public class WalkerDdDealer : Dealer<WalkerEnum, MonsterVariants>
    {
        [SerializeField] [CanBeNull] public Rigidbody body;
        [SerializeField] [CanBeNull] public Transform headTransform;
        [SerializeField] [CanBeNull] public MoveScript moveScript;
        [SerializeField] [CanBeNull] public Animator animator;
        [SerializeField] [CanBeNull] public Renderer[] witnessBlessing;
        [SerializeField] public float size = 1;
        [DoNotSerialize] public int group;
        [SerializeField] public (float current, float max) Health = (1f, 1f);

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
        [SerializeField] public TEnum type;
        
        public virtual void ApplyVariant(TVariant variant)
        {
        }
    }

    public class Dealer : MonoBehaviour
    {
        [SerializeField] [CanBeNull] public Transform mainTransform;
        [SerializeField] [CanBeNull] public NetworkObject networkObject;

        public virtual void ResetDealed(bool respawn)
        {
        } 
    }
}