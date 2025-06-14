using Shared.AnimParameter;
using UnityEngine;

namespace Monster.AnimParameter
{
    public class MonsterAnimP : WalkerAnimP
    {
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Dead = Animator.StringToHash("dead");
    }
}