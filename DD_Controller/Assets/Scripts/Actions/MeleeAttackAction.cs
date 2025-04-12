using UnityEngine;

namespace Actions
{
    public struct MeleeAttackAction : IDdAction
    {
        private readonly Animator _animator;
        private readonly LayerMask _layerMask;
        private readonly Transform _attackTransform;
        public int id { get; set; }
        private static readonly int Melee = Animator.StringToHash("Melee");
        private float _attackTimer;

        public MeleeAttackAction(Animator animator, Transform attackTransform, LayerMask mask)
        {
            id = 0;
            _animator = animator;
            _layerMask = mask;
            _attackTransform = attackTransform;
            _attackTimer = 0f;
        }

        public void launch()
        {
            _animator.SetTrigger(Melee);
            _attackTimer = Time.time + 0.25f;
        }

        public bool update()
        {
            if (Time.time < _attackTimer) return false;
            /*
            var dir = _attackTransform.rotation * Vector3.forward;
            if (Physics.SphereCast(_attackTransform.position - (2 * dir),
                    3f,
                    dir,
                    out var hitInfo,
                    5f,
                    _layerMask))
                hitInfo.rigidbody?.AddForce(_attackTransform.rotation * Vector3.forward * 100, ForceMode.Impulse);
            */

            return true;
        }

        public void end()
        {
        }
    }
}