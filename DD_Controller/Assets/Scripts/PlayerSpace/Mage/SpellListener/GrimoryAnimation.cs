using UnityEngine;
using UnityEngine.Serialization;
using PlayerSpace.Mage.UI;

namespace PlayerSpace.Mage.SpellListener
{
    public class GrimoryAnimation : MonoBehaviour
    {
        private const string INCANTING = "Incanting";
        private const string SPELL = "Spell";

        private static readonly int Spell1 = Animator.StringToHash(SPELL);
        private static readonly int Incanting1 = Animator.StringToHash(INCANTING);
        [FormerlySerializedAs("MageAnimator")] public Animator mageAnimator;

        [FormerlySerializedAs("GrimoryAnimator")]
        public Animator grimoryAnimator;

        [FormerlySerializedAs("MageQTEScript")]
        public MageQTEScript mageQTEScript;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            foreach (Spell spell in mageQTEScript.spellManager.GetSpells()) spell.AddSpellListener(Cast);
        }

        // Update is called once per frame
        private void Update()
        {
            bool b = mageQTEScript.GetIncantingState();
            mageAnimator.SetBool(Incanting1, b);
            grimoryAnimator.SetBool(Incanting1, b);
        }

        private void Cast(Spell spell)
        {
            mageAnimator.SetTrigger(Spell1);
            grimoryAnimator.SetBool(Incanting1, false);
        }
    }
}