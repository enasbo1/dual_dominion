using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class SpellLogger : MonoBehaviour
    {
        public SpellManager spellManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            foreach (Spell spell in spellManager.GetSpells()) spell.AddSpellListener(x => Debug.Log(x.name));
        }
    }
}