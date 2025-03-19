using Mage;
using UnityEditor.UI;
using UnityEngine;

namespace script.Mage.SpellListener
{
    public class GrimoryAnimation : MonoBehaviour
    {
        public Animator MageAnimator;
        public Animator GrimoryAnimator;
        public MageQTEScript MageQTEScript;

        private static string INCANTING = "Incanting";
        private static string SPELL = "Spell";
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            foreach (var spell in MageQTEScript.spellManager.GetSpells())
            {
                spell.AddSpellListener(Cast);                
            }
        
        }

        private void Cast(Spell spell)
        {
            MageAnimator.SetTrigger(SPELL);
            GrimoryAnimator.SetBool(INCANTING, false);
        }

        // Update is called once per frame
        void Update()
        {
            var b = MageQTEScript.GetIncantingState();
            MageAnimator.SetBool(INCANTING, b);
            GrimoryAnimator.SetBool(INCANTING, b);
        }
    }
}
