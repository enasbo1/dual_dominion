using System;
using Monster;
using Shared;
using UnityEngine;

namespace GameRule
{
    // WIP : knockBack non implementés
    // WIP : effect non implémentés
    public class LifeManager : WalkerManager
    {
        [SerializeField] private StandByManager<WalkerDdDealer, WalkerEnum, MonsterVariants> standByManager;
        private TableArray<(float current, float max)> _life = new(0);

        // Update is called once per frame
        void FixedUpdate()
        {
            for(int i = 0; i<Size; ++i) if (Active[i])
            {
                if (_life[i].current <= 0)
                {
                    standByManager.Kill(Elements[i]);
                }
            }
        }


        /*
         * target : la liste des gameObject touchés par l'attaque dans l'ordre d'application des dégats
         * damage : les dégats de bases
         * perforation : plus elle est élevée, moin les dégats sont réduits entre cheques cibles impactées
         */
        public void Hit(GameObject[] targets, float damage, int perforation = 1)
        {
            if (damage < 0) throw new ArgumentOutOfRangeException(nameof(damage));
            for (int i = 0; i < targets.Length; ++i)
            {
                GameObject go = targets[i];
                for(int j = 0; j < Size; ++j) if (Active[j] & (Elements[i].gameObject == go))
                {
                    (float current, float max) health = _life[j];
                    health.current -= damage;
                    health.current = Mathf.Clamp(health.current, 0, health.max);
                    float damageTaken = _life[i].current - health.current;
                    _life[j] = health;
                    
                    if (perforation!=0)
                        damage -= damageTaken / perforation;
                }
            }
        }
        
        protected override void AddChunk(int size)
        {
            _life.AddChunk(size);
        }

        protected override void AddElementInChunk(WalkerDdDealer element)
        {
            _life[Size] = element.Health;
        }

        protected override void AddElementInNew(WalkerDdDealer element)
        {
            _life.Add(element.Health);
        }
    }

    public enum Affect
    {
        Normal,
    }
}
