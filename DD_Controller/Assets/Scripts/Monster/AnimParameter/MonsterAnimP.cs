using Shared.AnimParameter;
using UnityEngine;

namespace Monster.AnimParameter
{
    public abstract class MonsterAnimP : WalkerAnimP
    {
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Dead = Animator.StringToHash("dead");
    }

    public abstract class WyvernAnimP : MonsterAnimP
    {
        public static readonly int Fly = Animator.StringToHash("Fly");
    }
}