using System;
using Monster.AnimParameter;
using Monster.Behavior;
using Shared;
using Unity.VisualScripting;
using UnityEngine;

namespace Monster
{
    public class MonsterDealer : WalkerDdDealer
    {
        
        [Header("Monster Characteristics")]
        [SerializeField] public float scoreValue = 5;
        [SerializeField] private float damages = 10f;
        [SerializeField] public MonsterBehaviorEnum[] behaviors;
        [SerializeField] public MonsterVariants[] variants;
        [SerializeField] public MonsterVariants curentVariant;
        
        [Header("Monster Body References")]
        [SerializeField] public SensorScript sensor;
        
        
        private bool _isDying = false;
        private float _timeToDie ;
        
        public float currentDamages
        {
            get { return processDamages(); }
        }

        public override void Kill()
        {
            animator.SetBool(MonsterAnimP.Dead, true);
            moveScript.canMove = false;
            moveScript.couldMove = false;
            _isDying = true;
            _timeToDie = Time.time + 1.5f;
        }
        public override void ApplyVariant(MonsterVariants variant)
        {
            curentVariant = variant;
            if (!mainTransform) return;
            mainTransform.localScale = Vector3.one * (variant == MonsterVariants.Big ? 1.4f : 1);

            
            if (body)
                body.mass = variant == MonsterVariants.Big ? 1.5f : 1;
        }

        private float processDamages()
        {
            return damages * (curentVariant == MonsterVariants.Big ? 1.5f : 1);
        }
        
        private void FixedUpdate()
        {
            if (_isDying && (_timeToDie < Time.time))
            {
                _isDying = false;            
                animator?.SetBool(MonsterAnimP.Dead, false);
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _isDying = false;            
        }
    }
}