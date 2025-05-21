using System;
using end_game;
using Shared;
using UnityEngine;

namespace Mage
{
    public class PlayerDealer : WalkerDdDealer
    {
        public float lifePoints;
        [SerializeField] private GameEnd gameEnd;
        private void Start()
        {
            lifePoints = maxHealth;
        }

        public void Aie(float damage)
        {
            lifePoints -= damage;
            animator?.SetTrigger(PlayerAnimP.Hurt);
            if (lifePoints <= 0)
            {
                gameEnd.GameOver();
            }
        }
    }
}