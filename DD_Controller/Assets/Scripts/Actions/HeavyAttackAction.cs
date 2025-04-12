using UnityEngine;

namespace Actions
{
    public struct HeavyAttackAction : IDdAction
    {
        private readonly Animator _animator;
        public int id { get; set; }

        private static readonly int Melee = Animator.StringToHash("melee");

        public HeavyAttackAction(Animator animator)
        {
            id = 0;
            _animator = animator;
        }

        public void launch()
        {
            _animator.SetTrigger(Melee);
        }

        public bool update()
        {
            return true;
        }

        public void end()
        {
        }
    }
}