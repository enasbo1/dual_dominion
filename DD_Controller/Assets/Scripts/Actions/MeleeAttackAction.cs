using GameRule;
using Monster;
using Move;
using UnityEngine;

namespace Actions
{
    public class MeleeAttackAction : IDdAction
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
            sensorScript.keepPresent = true;
        }

        public void launch()
        {
            if (Time.time < _attackTimer + .45f) return;
            if (_sensorScript.gameObject.activeSelf) return;
            _animator.SetTrigger(Melee);
            _attackTimer = Time.time + .3f;
            _sensorScript.gameObject.SetActive(true);

        }

        private void Hit(MonsterDealer dealer)
        {
            if (dealer.body)
                dealer.body.AddForce(
                    ((dealer.body.position - _attackTransform.position).normalized * 40) + Vector3.up * 10,
                    ForceMode.VelocityChange);
        }
        
        public bool update()
        {
            return !(Time.time < _attackTimer);
        }

        public void end()
        {
            _lifeManager.Hit(_sensorScript.nearby, _damage, onHit:Hit);
            _sensorScript.gameObject.SetActive(false);
        }
    }
}