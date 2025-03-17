using UnityEngine;

namespace Mage.SpellListener
{
    public class SpellLogger : MonoBehaviour
    {
        public SpellManager spellManager;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            foreach (Spell spell in spellManager.GetSpells())
            {
                spell.AddSpellListener(spell => Debug.Log(spell.name));
            }
        }
    }
}
