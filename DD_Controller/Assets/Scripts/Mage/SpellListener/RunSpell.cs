using Move;
using UnityEngine;
using UnityEngine.Serialization;

namespace Mage.SpellListener
{
    public class RunSpell : MonoBehaviour
    {
        public SpellManager spellManager;
        public FootMove footMoveScript;
        public Material effectMaterial;
        [FormerlySerializedAs("effectBarrer")] public SkinnedMeshRenderer effectRenderer;
        private bool _active;

        private float _initialValue;
        private Material _originalMaterial;

        private float _timer;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _initialValue = footMoveScript.movementSpeed;
            _originalMaterial = effectRenderer.material;
            spellManager.GetSpellById(1).AddSpellListener(OnSpell);
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!_active) return;
            _timer -= Time.fixedDeltaTime;
            if (_timer < 0)
            {
                footMoveScript.movementSpeed = _initialValue;
                effectRenderer.material = _originalMaterial;
                _active = false;
                spellManager.GetSpellById(1).isInCast = false;
            }
        }

        private void OnSpell(Spell spell)
        {
            _timer = 5;
            if (_active) return;
            _initialValue = footMoveScript.movementSpeed;
            footMoveScript.movementSpeed = _initialValue * 2;
            effectRenderer.material = effectMaterial;
            _active = true;
            spell.isInCast = true;
        }
    }
}