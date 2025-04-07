using UnityEngine;

namespace actions
{
    public struct MeleeAttackAction : IDdAction
    {
        private Animator _animator;
        public int id { get;  set;}
        
        private static readonly int Melee = Animator.StringToHash("Melee");

        MeleeAttackAction(Animator animator)
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