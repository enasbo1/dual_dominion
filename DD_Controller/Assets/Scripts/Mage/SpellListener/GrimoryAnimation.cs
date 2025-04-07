using UnityEngine;
using UnityEngine.Serialization;

namespace Mage.SpellListener
{
    public class GrimoryAnimation : MonoBehaviour
    {
        [FormerlySerializedAs("MageAnimator")] public Animator mageAnimator;
        [FormerlySerializedAs("GrimoryAnimator")] public Animator grimoryAnimator;
        [FormerlySerializedAs("MageQTEScript")] public MageQTEScript mageQTEScript;

        private const string INCANTING = "Incanting";
        private const string SPELL = "Spell";
        
        private static readonly int Spell1 = Animator.StringToHash(SPELL);
        private static readonly int Incanting1 = Animator.StringToHash(INCANTING);

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            foreach (var spell in mageQTEScript.spellManager.GetSpells())
            {
                spell.AddSpellListener(Cast);                
            }
        
        }

        private void Cast(Spell spell)
        {
            mageAnimator.SetTrigger(Spell1);
            grimoryAnimator.SetBool(Incanting1, false);
        }

        // Update is called once per frame
        void Update()
        {
            var b = mageQTEScript.GetIncantingState();
            mageAnimator.SetBool(Incanting1, b);
            grimoryAnimator.SetBool(Incanting1, b);
        }
    }
}
