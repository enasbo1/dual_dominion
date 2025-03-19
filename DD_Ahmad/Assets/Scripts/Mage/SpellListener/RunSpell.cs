using Mage;
using UnityEngine;

namespace script.Mage.SpellListener
{
    public class RunSpell : MonoBehaviour
    {
        public SpellManager spellManager;
        public FootMove footMoveScript;
        public Material effectMaterial;
        public SkinnedMeshRenderer effectBarrer;
        
        private float _timer;

        private float _initialValue;
        private Material _originalMaterial;
        private bool _active;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _initialValue = footMoveScript.MoveSpeed;
            _originalMaterial = effectBarrer.material;
            spellManager.GetSpellById(1).AddSpellListener(OnSpell);
        }

        private void OnSpell(Spell spell)
        {
            _timer = 5;
            if (_active) return;
            _initialValue = footMoveScript.MoveSpeed;
            footMoveScript.MoveSpeed = _initialValue*2;
            effectBarrer.material = effectMaterial;
            _active = true;
        }
        // Update is called once per frame
        private void FixedUpdate()
        {
            if (!_active) return;
            _timer -= Time.fixedDeltaTime;
            if (_timer < 0)
            {
                footMoveScript.MoveSpeed = _initialValue;
                effectBarrer.material = _originalMaterial;
                _active = false;
            }
        }
    }
}
