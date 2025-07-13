using GoD;
using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerSpace.Mage.SpellListener
{
    public class Konami : MonoBehaviour
    {
        public SpellManager spellManager;
        [FormerlySerializedAs("mageDealer")] public MageDealer mageDealer;
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
            if (mageDealer != null)
            {
                mageDealer.skillsToUnlock += 1;
            }

            if (godManager != null)
            {
                godManager.karmaPoint += 1500;
            }
        }
    }
}