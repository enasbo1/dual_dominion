using UnityEngine;

namespace Actions
{
    public class ShieldAction : IDdAction
    {
        private readonly Animator _animator;
        private readonly SensorScript _sensor;
        private readonly Transform _ownerTransform;
        public int id { get; set; }
        private static readonly int Shield = Animator.StringToHash("shield");
        private bool Active;

        public ShieldAction(Animator animator, Transform ownerTransform, SensorScript sensor)
        {
            id = 0;
            _animator = animator;
            Active = false;
            _sensor = sensor;
            _ownerTransform = ownerTransform;
            _sensor.Listener += Push;
        }

        public void launch()
        {
            _animator.SetBool(Shield, true);
            _sensor.gameObject.SetActive(true);
            Active = true;
        }

        private void Push(Collider collider)
        {
            if (!Active) return;
            if (collider.attachedRigidbody)
                collider.attachedRigidbody.AddForce(
                    ((collider.attachedRigidbody.position - _ownerTransform.position).normalized * 15) + Vector3.up * 10,
                    ForceMode.Impulse);
        }
            
        public bool update()
        {
            if (!_sensor.gameObject.activeInHierarchy)
                _sensor.gameObject.SetActive(true);
            return true;
        }

        public void end()
        {
            _animator.SetBool(Shield, false);
            _sensor.gameObject.SetActive(false);
            Active = false;
        }
    }
}