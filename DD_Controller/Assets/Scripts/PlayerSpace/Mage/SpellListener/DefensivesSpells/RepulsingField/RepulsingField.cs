using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class RepulsingField : MonoBehaviour
    {
        public SpellManager spellManager;
        public Transform repulsingShieldTransform;
        
        private bool _castAsFailure;
        private float growMultiplayer = 1f;
        private float _timer;
        
        private Spell _repulsingField;

        private void Start()
        {
            _repulsingField = spellManager.GetSpellByName("Repulsing Field");
            _repulsingField.AddSpellListener(_ => OnSpellCast());
            _repulsingField.AddSpellFailureListener(_ => OnSpellCastAsFailure());
            
            repulsingShieldTransform.gameObject.SetActive(false);
        }

        private void CastEnd()
        {
            _repulsingField.isInCast = false;
            
            repulsingShieldTransform.gameObject.SetActive(false);
            repulsingShieldTransform.localScale = Vector3.one;
            growMultiplayer = 1f;
        }

        private void FieldGrowth()
        {
            float sizeToAdd = (0.05f / (growMultiplayer * growMultiplayer + 1f)) * growMultiplayer;

            float newScale = repulsingShieldTransform.localScale.x + sizeToAdd;
            repulsingShieldTransform.localScale = new Vector3(newScale, newScale, newScale);
        }
        
        private void FixedUpdate()
        {
            if (!_repulsingField.isInCast) return;
            
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                CastEnd();
                return;
            }

            FieldGrowth();
        }

        private void OnSpellCast()
        {
            _repulsingField.isInCast = true;
            _castAsFailure = false;

            repulsingShieldTransform.gameObject.SetActive(true);
            _timer = 15f;
        }
        
        private void OnSpellCastAsFailure()
        {
            _repulsingField.isInCast = true;
            _castAsFailure = true;
            
            repulsingShieldTransform.gameObject.SetActive(true);
            _timer = 3f;
        }
    }
}