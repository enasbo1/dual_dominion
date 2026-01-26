using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class Healing : MonoBehaviour
    {
        public SpellManager spellManager;
        public MageDealer mageDealer;
        
        private Spell _healing;
        private Spell _healing2;

        private void Start()
        {
            _healing2 = spellManager.GetSpellByName("Healing");
            _healing = spellManager.GetSpellByName("Healing 2");
            _healing.AddSpellListener(OnSpellCast);
            _healing.AddSpellFailureListener(OnSpellCastAsFailure);
        }

        private void OnSpellCast(Spell spell)
        {
            mageDealer.lifePoints += 100f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }

            _healing2.cooldown = 0;
            mageDealer.UpdateHealthUI();
        }
        
        private void OnSpellCastAsFailure(Spell spell)
        {
            mageDealer.lifePoints += 10f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }
            
            mageDealer.UpdateHealthUI();
        }
    }
}