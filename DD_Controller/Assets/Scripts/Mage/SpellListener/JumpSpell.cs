using UnityEngine;
using UnityEngine.Serialization;

namespace Mage.SpellListener
{
    public class JumpSpell : MonoBehaviour
    {
        public SpellManager spellManager;
        [FormerlySerializedAs("characterRigidbody")] public Rigidbody characterRigidBody;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            spellManager.GetSpellById(5).AddSpellListener(OnSpell);
        }

        private void OnSpell(Spell spell)
        {
            characterRigidBody.AddForce((Vector3.up * 80), ForceMode.Impulse);
        }
    }
}
