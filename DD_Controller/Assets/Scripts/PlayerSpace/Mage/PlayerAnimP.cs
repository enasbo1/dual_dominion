using Shared.AnimParameter;
using UnityEngine;

namespace PlayerSpace.Mage
{
    public abstract class PlayerAnimP : WalkerAnimP
    {
        public static readonly int Hurt = Animator.StringToHash("hurt");
    }
}