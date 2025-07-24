using PlayerSpace.GoD;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class Konami : MonoBehaviour
    {
        public SpellManager spellManager;
        public MageDealer mageDealer;
        public GodManagerScript godManager;

        private float _initialValue;
        private Material _originalMaterial;
        
        private Spell _konamiSpell;

        private void Start()
        {
            _konamiSpell = spellManager.GetSpellById(4);
            _konamiSpell.AddSpellListener(OnSpell);
        }

        private void OnSpell(Spell spell)
        {
            if (mageDealer && spellManager.spellsToUnlock.Count > 0)
            {
                mageDealer.skillsToUnlock += 1;
            }

            if (godManager)
            {
                godManager.karmaPoint += 1500;
            }
        }
    }
}