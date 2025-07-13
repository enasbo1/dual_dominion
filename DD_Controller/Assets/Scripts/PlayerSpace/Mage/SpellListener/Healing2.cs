using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerSpace.Mage.SpellListener
{
    public class Healing2 : MonoBehaviour
    {
        public SpellManager spellManager;
        [FormerlySerializedAs("mageDealer")] public MageDealer mageDealer;
        private bool _active;
        
        private Spell _healingSpell;
        private Spell _healingSpell2;

        private void Start()
        {
            _healingSpell = spellManager.GetSpellById(10);
            _healingSpell2 = spellManager.GetSpellById(11);
            _healingSpell2.AddSpellListener(OnSpell);
            _healingSpell2.AddSpellFailureListener(OnSpellFailure);
        }

        private void OnSpell(Spell spell)
        {
            mageDealer.lifePoints += 250f;
            if (mageDealer.lifePoints > mageDealer.maxHealth)
            {
                mageDealer.lifePoints = mageDealer.maxHealth;
            }

            _healingSpell.cooldown = 0;
            mageDealer.UpdateHealthUI();
        }
        
        private void OnSpellFailure(Spell spell)
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