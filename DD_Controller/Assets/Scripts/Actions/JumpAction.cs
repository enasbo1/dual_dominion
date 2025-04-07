using UnityEngine;

namespace Actions
{
    public struct JumpAction : IDdAction
    {
        private readonly Rigidbody _rb;
        private readonly Animator _animator;
        private readonly float _jumpForce;
        public int id { get;  set;}
        
        private static readonly int Jump = Animator.StringToHash("Jump");
        private float _jumpTimer;

        public JumpAction(Rigidbody rb, Animator animator, float jumpForce)
        {
            id = 0;
            _rb = rb;
            _animator = animator;
            _jumpForce = jumpForce;
            _jumpTimer = 0f;
        }


        public void launch()
        {
            _jumpTimer = Time.time;
            _animator.SetTrigger(Jump);
        }

        public bool update()
        {
            if (Time.time < _jumpTimer + 0.1f) return false;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            return true;
        }

        public void end()
        {
        }
    }
}