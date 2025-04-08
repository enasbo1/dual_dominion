using System;
using Move;
using UnityEngine;

namespace Mage.SpellListener
{
    public class SkyView : MonoBehaviour
    {
        public GameObject cameraDefaultScripts;
        public MageCameraManager playerCameraManager;
        public SpellManager spellManager;
        
        [Range(0.1f, 3f)]
        public float transitionSpeed = 0.5f;
        
        private float _timer;
        private float _timeLimit;
        
        public float CAMERA_MAX_HEIGHT = 40f;
        private float _cameraPositionY;
        
        private Quaternion playerCamera;
        private float _cameraRotationY;
        private float _cameraRotationX;
    
        private Spell _skyView;
    
        private void SpellCasted()
        {
            _skyView.isInCast = true;
            _timeLimit = 10f;
            _timer = _timeLimit;
            
            playerCameraManager.SetRotable(false);
        }
        
        private void CastEnd()
        {
            if (_cameraPositionY > 0)
            {
                _cameraPositionY -= transitionSpeed;
                playerCameraManager.SetNewHeight(_cameraPositionY);
                return;
            }
            
            playerCameraManager.SetRotable(true);
            _skyView.isInCast = false;
        }
    
        void Start()
        {
            _skyView = spellManager.GetSpellById(2);

            _skyView.AddSpellListener(_ => SpellCasted());
        }
    
        void FixedUpdate()
        {
            if (!_skyView.isInCast) return;
            
            if (_timer <= 0)
            {
                CastEnd();
                return;
            }
            
            if (_cameraPositionY < CAMERA_MAX_HEIGHT)
            {
                _cameraPositionY += transitionSpeed;
                playerCameraManager.SetNewHeight(_cameraPositionY);
            }

            playerCamera = playerCameraManager.GetCameraRotation();
            Vector3 eulerCamera = playerCamera.eulerAngles;
            
            float currentX = eulerCamera.x > 180 ? eulerCamera.x - 360 : eulerCamera.x;
            if (Mathf.Abs(currentX - 85f) > 0.2f)
            {
                float newX = currentX > 85f ? currentX - transitionSpeed : currentX + transitionSpeed;
                playerCameraManager.SetRotationX(newX);
            }
            
            float currentY = eulerCamera.y > 180 ? eulerCamera.y - 360 : eulerCamera.y;
            if (Mathf.Abs(currentY - 0f) > 0.2f)
            {
                float newY = currentY > 0f ? currentY - transitionSpeed : currentY + transitionSpeed;
                playerCameraManager.SetRotationY(newY);
            }
            
            _timer -= Time.deltaTime;
        }
    }
}
