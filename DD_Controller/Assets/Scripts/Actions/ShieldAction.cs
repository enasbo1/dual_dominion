using UnityEngine;

namespace Actions
{
    public struct ShieldAction : IDdAction
    {
        private readonly Animator _animator;
        public int id { get; set; }
        private static readonly int Shield = Animator.StringToHash("shield");
        public bool Active;

        public ShieldAction(Animator animator)
        {
            id = 0;
            _animator = animator;
            Active = true;
        }

        public void launch()
        {
            _animator.SetBool(Shield, true);
        }

        public bool update()
        {
            return true;
        }

        public void end()
        {
            _animator.SetBool(Shield, false);
        }
    }
}