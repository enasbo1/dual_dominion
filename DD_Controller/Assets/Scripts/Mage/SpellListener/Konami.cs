using GoD;
using UnityEngine;

namespace Mage.SpellListener
{
    public class Konami : MonoBehaviour
    {
        public SpellManager spellManager;
        public PlayerDealer playerDealer;
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
            if (playerDealer != null)
            {
                playerDealer.skillsToUnlock += 1;
            }

            if (godManager != null)
            {
                godManager.karmaPoint += 1500;
            }
        }
    }
}