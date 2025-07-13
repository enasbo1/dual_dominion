using System;
using System.Collections.Generic;
using Monster;
using Shared;
using UnityEngine;

namespace GameRule
{
    public class LifeManager : LifeManager<WalkerDdDealer, WalkerEnum, MonsterVariants>
    {
    }
    
    // WIP : knockBack non implementés
    // WIP : effect non implémentés
    public class LifeManager<TDealer, TEnum, TVariant> : Manager<TDealer, TEnum, TVariant> where TDealer : WalkerDealer<TEnum, TVariant> where TEnum : Enum where TVariant : Enum
    {
        public static LifeManager<TDealer, TEnum, TVariant> MainInstance = null;
        [SerializeField] protected bool IsMainInstance = false;
        [SerializeField] private StandByManager<TDealer, TEnum, TVariant> standByManager;
        private TableArray<(float current, float max)> _life = new TableArray<(float current, float max)>(0);

        protected virtual void Start()
        {
            if (!IsMainInstance) return;
            if (LifeManager<TDealer, TEnum, TVariant>.MainInstance != null)
                throw new Exception("there is More than one instance of LifeManager marked as the Main Instance");
            LifeManager<TDealer, TEnum, TVariant>.MainInstance = this;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            for(int i = 0; i<Size; ++i) if (Active[i])
            {
                if (_life[i].current <= 0)
                {
                    onDeath(Elements[i]);
                    standByManager.Kill(Elements[i]);
                }
            }
        }

        protected virtual void onDeath(TDealer dealer)
        {
        }


        /*
         * target : la liste des gameObject touchés par l'attaque dans l'ordre d'application des dégats
         * damage : les dégats de bases
         * perforation : plus elle est élevée, moin les dégats sont réduits entre cheques cibles impactées
         */
        public void Hit(GameObject[] targets, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            for (int i = 0; i < targets.Length; ++i)
            {
                float damageTaken = Hit(targets[i], damage, perforation);
                if (perforation!=0)
                    damage -= damageTaken / perforation;
            }
        }
        
        public void Hit(Rigidbody[] targets, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            for (int i = 0; i < targets.Length; ++i)
            {
                float damageTaken = Hit(targets[i], damage, perforation);                
                if (perforation!=0)
                    damage -= damageTaken / perforation;
            }
        }
        public void Hit(List<Collider> targets, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            for (int i = 0; i < targets.Count; ++i)
            {
                float damageTaken = Hit(targets[i].attachedRigidbody, damage, perforation, onHit);                
                if (perforation!=0)
                    damage -= damageTaken / perforation;
            }
        }
        public float Hit(GameObject targets, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {
            for(int j = 0; j < Size; ++j) if (Active[j] & (Elements[j].gameObject == targets))
            {
                return hit(j, damage, perforation);
            }

            return 0f;
        }
        
        public float Hit(Rigidbody targets, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {

            for(int j = 0; j < Size; ++j) if (Active[j] & (Elements[j].body == targets))
            {
                return hit(j, damage, perforation, onHit);
            }

            return 0f;
        }

        private float hit(int index, float damage, int perforation = 1, Action<TDealer> onHit = null)
        {
            (float current, float max) health = _life[index];
            health.current -= damage;
            health.current = Mathf.Clamp(health.current, 0, health.max);
                
            float damageTaken = _life[index].current - health.current;
            _life[index] = health;
            
            onHit?.Invoke(Elements[index]);
            
            return damageTaken;
        }
        
        protected override void AddChunk(int size)
        {
            _life.AddChunk(size);
        }
        protected override void AddElementInChunk(TDealer element)
        {
            _life[Size] = (element.maxHealth, element.maxHealth);
        }

        protected override void AddElementInNew(TDealer element)
        {
            _life.Add((element.maxHealth, element.maxHealth));
        }

        protected override void RestoreElement(int i, TDealer element)
        {
            _life[i] = (element.maxHealth, element.maxHealth);
        }
    }

    public enum Affect
    {
        Normal,
    }
}
