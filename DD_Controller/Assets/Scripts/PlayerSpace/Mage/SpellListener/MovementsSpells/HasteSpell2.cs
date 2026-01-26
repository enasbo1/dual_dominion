using Move;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class HasteSpell2 : MonoBehaviour
    {
        public SpellManager spellManager;
        public FootMove footMoveScript;
        public Material effectMaterial;
        public Material effectFailureMaterial;
        public Material effectRecastFailureMaterial;
        public SkinnedMeshRenderer effectRenderer;

        private float _initialValue;
        private Material _originalMaterial;

        private float _timer;
        
        private Spell _haste;
        private Spell _haste2;
        private Spell _haste3;

        private void Start()
        {
            _haste = spellManager.GetSpellByName("Haste");
            _haste2 = spellManager.GetSpellByName("Haste 2");
            _haste3 = spellManager.GetSpellByName("Haste 3");
            
            _initialValue = footMoveScript.movementSpeed;
            _originalMaterial = effectRenderer.material;
            
            _haste2.AddSpellListener(_ => OnSpellCast());
            _haste2.AddSpellFailureListener(_ => OnSpellCastAsFailure());
            _haste.isInCast = false;
            _haste3.isInCast = false;
        }
        
        private void FixedUpdate()
        {
            if (!_haste2.isInCast) return;
            _timer -= Time.fixedDeltaTime;

            if (_timer > 0f) return;
            footMoveScript.movementSpeed = _initialValue;
            effectRenderer.material = _originalMaterial;
            _haste2.isInCast = false;
        }

        private void OnSpellCast()
        {
            _timer = 22.5f;
            footMoveScript.movementSpeed = _initialValue * 3f;
            effectRenderer.material = effectMaterial;
            _haste2.isInCast = true;
        }
        
        private void OnSpellCastAsFailure()
        {
            _timer = 6f;
            footMoveScript.movementSpeed = _haste.isInCast ? _initialValue * 0.6f : _initialValue * 1.75f;
            effectRenderer.material =  _haste.isInCast ? effectRecastFailureMaterial : effectFailureMaterial;
            _haste.cooldown = 3f;
            _haste2.cooldown = 4f;
            _haste3.cooldown = 5f;
        }
    }
}