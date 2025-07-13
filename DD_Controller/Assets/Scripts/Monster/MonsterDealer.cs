using System;
using System.Collections.Generic;
using Monster.AnimParameter;
using Monster.Behavior;
using Monster.Variants;
using Shared;
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
        
        [Header("Monster Variant attributes")]
        [SerializeField] private VariantApplier<MonsterDealer>[] appliers;
        [SerializeField] private List<GameObject> firstVariant;
        [SerializeField] private List<GameObject> secondVariant;
        [SerializeField] private List<GameObject> thirdVariant;
        [SerializeField] private List<GameObject> fourthVariant;
        [SerializeField] private List<GameObject> fifthVariant;
        
        
        private bool _isDying = false;
        private float _timeToDie ;
        private List<GameObject>[] _variantAttributes;
        
        private readonly List<Action<MonsterDealer, Rigidbody>> _hitEvents =
            new List<Action<MonsterDealer, Rigidbody>>();
        public event Action<MonsterDealer, Rigidbody> HitListener
        {
            add => _hitEvents.Add(value);
            remove => _hitEvents.Remove(value);
        }
        
        private void Awake()
        {
            _variantAttributes = new []
            {
                firstVariant,
                secondVariant,
                thirdVariant,
                fourthVariant,
                fifthVariant
            };
            sensor.Listener += collider => onHit(collider.attachedRigidbody);
        }

        public float currentDamages => processDamages();

        private void onHit(Rigidbody rb)
        {
            if (!rb) return;
            
            foreach (Action<MonsterDealer, Rigidbody> action in _hitEvents)
                action.Invoke(this, rb);
        }

        public override void Kill()
        {
            animator.SetBool(MonsterAnimP.Dead, true);
            moveScript.canMove = false;
            moveScript.couldMove = false;
            _isDying = true;
            _timeToDie = Time.time + 1.5f;
        }

        private float _defaultMoveSpeed;
        private float _defaultMaxHealth;
        
        public override void ApplyVariant(MonsterVariants variant)
        {
            curentVariant = variant;
            if (!mainTransform) return;
            

            int vIndex;

            for (vIndex = 0; vIndex < variants.Length; vIndex++)
            {
                if (variant == variants[vIndex]) break;
            }

            if (vIndex < variants.Length)
            {
                for (int i = 0; i < 5; ++i)
                {
                    if (i == vIndex) continue;
                    if (_variantAttributes != null)
                    {
                        foreach (GameObject go in _variantAttributes[i])
                        {
                            go.SetActive(false);
                        }
                    }
                }
                
                for (int i = 0; i < 5; ++ i)
                {
                    if (i != vIndex) continue;
                    if (_variantAttributes != null)
                    {
                        foreach (GameObject go in _variantAttributes[i])
                        { 
                            go.SetActive(true);
                        }
                    }
                    
                    break;
                }
            }

            for (int i = 0; i < appliers.Length; ++i)
                appliers[i].RestoreDefault(this);
            
            mainTransform.localScale = Vector3.one * (variant == MonsterVariants.Big ? 2f : 1);

            if (body)
                body.mass *= variant == MonsterVariants.Big ? 4f : 1;

            for (int i = 0; i < appliers.Length; ++i)
                appliers[i].ApplyVariant(this, variant);
        }

        private float processDamages()
        {
            return damages * 
                   (curentVariant == MonsterVariants.DProtector ? 1.2f : 1f) *
                   (curentVariant == MonsterVariants.Big ? 1.5f : 1);
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