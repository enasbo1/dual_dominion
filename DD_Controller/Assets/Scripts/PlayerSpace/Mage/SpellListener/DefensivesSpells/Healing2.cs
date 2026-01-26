using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class Healing2 : MonoBehaviour
    {
        public SpellManager spellManager;
        public MageDealer mageDealer;
        
        private Spell _healing;
        private Spell _healing2;

        private void Start()
        {
            _healing = spellManager.GetSpellByName("Healing");
            _healing2 = spellManager.GetSpellByName("Healing 2");
            _healing2.AddSpellListener(OnSpellCast);
            _healing2.AddSpellFailureListener(OnSpellCastAsFailure);
        }

        private void OnSpellCast(Spell spell)
        {
            mageDealer.lifePoints += 250f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }

            _healing.cooldown = 0;
            mageDealer.UpdateHealthUI();
        }
        
        private void OnSpellCastAsFailure(Spell spell)
        {
            mageDealer.lifePoints += 25f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }
            
            mageDealer.UpdateHealthUI();
        }
    }
}