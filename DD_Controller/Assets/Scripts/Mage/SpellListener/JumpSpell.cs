using Mage;
using UnityEngine;

namespace script.Mage.SpellListener
{
    public class JumpSpell : MonoBehaviour
    {
        public SpellManager spellManager;
        public Rigidbody characterRigidbody;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spellManager.GetSpellById(5).AddSpellListener(OnSpell);
        }

        private void OnSpell(Spell spell)
        {
            characterRigidbody.AddForce((Vector3.up * 80), ForceMode.Impulse);
        }
    }
}
