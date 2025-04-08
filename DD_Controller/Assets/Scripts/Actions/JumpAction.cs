using UnityEngine;

namespace Actions
{
    public struct JumpAction : IDdAction
    {
        private readonly Rigidbody _rb;
        private readonly float _jumpForce;
        public int id { get;  set;}
        
        private float _jumpTimer;
        private Vector3 _lastPosition;
        private Vector3 _velocity;
        public JumpAction(Rigidbody rb , float jumpForce)
        {
            id = 0;
            _rb = rb;
            _jumpForce = jumpForce;
            _jumpTimer = 0f;
            _velocity = Vector3.zero;
            _lastPosition = rb.transform.position;
        }


        public void launch()
        {
            _jumpTimer = Time.time;
            _lastPosition = _rb.transform.position;
        }

        public bool update()
        {
            var v = _rb.transform.position - _lastPosition;
            _lastPosition = _rb.transform.position;
            if (_velocity.sqrMagnitude < v.sqrMagnitude)
                _velocity = v;
            if (Time.time < _jumpTimer + 0.2f) return false;
            _rb.linearVelocity = _velocity/Time.deltaTime;
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            return true;
        }

        public void end()
        {
        }
    }
}