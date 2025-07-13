using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace PlayerSpace.Mage.SpellListener
{
    public class JumpSpell : MonoBehaviour
    {
        public SpellManager spellManager;

        [FormerlySerializedAs("characterRigidbody")]
        public Rigidbody characterRigidBody;

        public Material effectMaterial;

        public PlayerInput playerInputs;

        public Renderer effectBarer;
        private InputAction _action;
        private bool _active;

        private float _initialValue;
        private bool _is_jumping;
        private Material _originalMaterial;

        private float _timer;

        private void Start()
        {
            spellManager.GetSpellById(5).AddSpellListener(OnSpell);
            playerInputs.actions["Jump"].performed += _ => _is_jumping = true;
            playerInputs.actions["Jump"].canceled += _ => _is_jumping = false;
        }

        private void FixedUpdate()
        {
            if (!_active) return;

            Vector3 v = characterRigidBody.linearVelocity;
            v.y = v.y < -2f ? -2f : v.y;
            if (_is_jumping) v.y += 25f * Time.fixedDeltaTime;
            characterRigidBody.linearVelocity = v;

            if (Time.time < _timer) return;
            effectBarer.material = _originalMaterial;
            _active = false;
        }

        private void OnSpell(Spell spell)
        {
            _timer = Time.time + 10f;
            if (_active) return;

            _originalMaterial = effectBarer.material;
            effectBarer.material = effectMaterial;
            _active = true;
        }
    }
}