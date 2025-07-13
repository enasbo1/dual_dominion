using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerSpace.Mage.SpellListener
{
    public class Healing : MonoBehaviour
    {
        public SpellManager spellManager;
        [FormerlySerializedAs("mageDealer")] public MageDealer mageDealer;
        private bool _active;
        
        private Spell _healingSpell;
        private Spell _healingSpell2;

        private void Start()
        {
            _healingSpell2 = spellManager.GetSpellById(11);
            _healingSpell = spellManager.GetSpellById(10);
            _healingSpell.AddSpellListener(OnSpell);
            _healingSpell.AddSpellFailureListener(OnSpellFailure);
        }

        private void OnSpell(Spell spell)
        {
            mageDealer.lifePoints += 100f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }

            _healingSpell2.cooldown = 0;
            mageDealer.UpdateHealthUI();
        }
        
        private void OnSpellFailure(Spell spell)
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