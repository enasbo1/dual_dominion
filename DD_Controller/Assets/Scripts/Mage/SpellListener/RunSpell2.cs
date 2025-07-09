using Move;
using UnityEngine;

namespace Mage.SpellListener
{
    public class RunSpell2 : MonoBehaviour
    {
        public SpellManager spellManager;
        public FootMove footMoveScript;
        public Material effectMaterial;
        public Material effectFailureMaterial;
        public SkinnedMeshRenderer effectRenderer;
        private bool _active;

        private float _initialValue;
        private Material _originalMaterial;

        private float _timer;
        
        private Spell _runSpell2;

        private void Start()
        {
            _runSpell2 = spellManager.GetSpellById(9);
            _initialValue = footMoveScript.movementSpeed;
            _originalMaterial = effectRenderer.material;
            _runSpell2.AddSpellListener(OnSpell);
            _runSpell2.AddSpellFailureListener(OnSpellFailure);
        }
        
        private void FixedUpdate()
        {
            if (!_active) return;
            _timer -= Time.fixedDeltaTime;
            if (_timer < 0)
            {
                footMoveScript.movementSpeed = _initialValue;
                effectRenderer.material = _originalMaterial;
                _active = false;
                _runSpell2.isInCast = false;
            }
        }

        private void OnSpell(Spell spell)
        {
            _timer = 15;
            footMoveScript.movementSpeed = _initialValue * 5f;
            effectRenderer.material = effectMaterial;
            _active = true;
            spell.isInCast = true;
        }
        
        private void OnSpellFailure(Spell spell)
        {
            _timer = 5;
            footMoveScript.movementSpeed = _initialValue * 2.5f;
            effectRenderer.material = effectFailureMaterial;
            _active = true;
        }
    }
}