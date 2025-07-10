using UnityEngine;

namespace Mage.SpellListener
{
    public class Healing : MonoBehaviour
    {
        public SpellManager spellManager;
        public PlayerDealer playerDealer;
        private bool _active;
        
        private Spell _healingSpell;

        private void Start()
        {
            _healingSpell = spellManager.GetSpellById(10);
            _healingSpell.AddSpellListener(OnSpell);
            _healingSpell.AddSpellFailureListener(OnSpellFailure);
        }

        private void OnSpell(Spell spell)
        {
            playerDealer.lifePoints += 200f;
            if (playerDealer.lifePoints > playerDealer.maxHealth)
            {
                playerDealer.lifePoints = playerDealer.maxHealth;
            }
        }
        
        private void OnSpellFailure(Spell spell)
        {
            playerDealer.lifePoints += 50f;
            if (playerDealer.lifePoints > playerDealer.maxHealth)
            {
                playerDealer.lifePoints = playerDealer.maxHealth;
            }
        }
    }
}