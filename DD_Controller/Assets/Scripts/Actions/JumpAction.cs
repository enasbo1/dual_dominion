using Move;
using UnityEngine;

namespace Actions
{
    public struct JumpAction : IDdAction
    {
        private readonly Rigidbody _rb;
        private readonly float _jumpForce;
        public int id { get; set; }

        private float _jumpTimer;
        private Vector3 _velocity;
        private readonly MoveScript _moveScript;
        private bool _hasJumped;

        public JumpAction(Rigidbody rb, float jumpForce, MoveScript moveScript)
        {
            id = 0;
            _rb = rb;
            _jumpForce = jumpForce;
            _moveScript = moveScript;
            _jumpTimer = 0f;
            _velocity = Vector3.zero;
            _hasJumped = false;
        }


        public void launch()
        {
            _jumpTimer = Time.time;
            _velocity = Vector3.zero;
            _jumpTimer = 0f;
            _hasJumped = false;
        }

        public bool update()
        {
            Vector3 v = _moveScript.GetMove();
            if (_velocity.sqrMagnitude < v.sqrMagnitude)
                _velocity = v;
            if (Time.time < _jumpTimer + 0.2f) return false;

            if (_hasJumped)
            {
                bool midAir = !_moveScript.canMove;
                _moveScript.canMove = false;
                return  midAir | (Time.time > _jumpTimer + 1f);
            }
            
            _rb.linearVelocity = _velocity;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            _hasJumped = true;
            return false;
        }

        public void end()
        {
        }
    }
}