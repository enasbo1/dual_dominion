using GameRule;
using Monster;
using Move;
using UnityEngine;

namespace Actions
{
    public struct MeleeAttackAction : IDdAction
    {
        private readonly Animator _animator;
        private readonly MonsterLifeManager _lifeManager;
        private readonly Transform _attackTransform;
        private readonly float _damage;
        private readonly SensorScript _sensorScript;
        public int id { get; set; }
        private static readonly int Melee = Animator.StringToHash("Melee");
        private float _attackTimer;

        public MeleeAttackAction(Animator animator, Transform attackTransform, SensorScript sensorScript, MonsterLifeManager lifeManager, float damage = 10f)
        {
            id = 0;
            _animator = animator;
            _lifeManager = lifeManager;
            _attackTransform = attackTransform;
            _attackTimer = 0f;
            _damage = damage;
            _sensorScript = sensorScript;
            sensorScript.Listener += Damage;
        }

        public void launch()
        {
            if (Time.time < _attackTimer + .75f) return;
            if (_sensorScript.gameObject.activeSelf) return;
            _animator.SetTrigger(Melee);
            _attackTimer = Time.time + .75f;
            _sensorScript.gameObject.SetActive(true);

        }

        private void Damage(Collider collider)
        {

            if (_attackTimer > Time.time) return;
            
            _lifeManager.Hit(collider.attachedRigidbody, _damage);
            collider.attachedRigidbody.AddForce((collider.attachedRigidbody.position-_attackTransform.position).normalized * 100, ForceMode.Impulse);

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
            _sensorScript.gameObject.SetActive(false);
        }
    }
}