using Move;
using UnityEngine;

namespace Mage.SpellListener
{
    public class SkyView : MonoBehaviour
    {
        public SpellManager spellManager;
        public Transform playerCharacterController;
        public MageCameraManager playerCameraManager;
        public GameObject movementScripts2D;
        public GameObject movementScripts3D;
        public Vector3 targetPosition = new Vector3(0, 40, -4);
        public Vector3 targetEulerRotation = new Vector3(85, 0, 0);

        private Spell _skyView;
        
        private float _timer;
        private float _timeLimit;
        
        private Transform _directionMain;
        private Transform _playerCamera;
        
        private Vector3 _playerCameraDefaultPosition;
        private Quaternion _playerCameraDefaultRotation;
    
        private void SpellCasted()
        {
            _skyView.isInCast = true;
            
            _timeLimit = 20f;
            _timer = _timeLimit;
            
            movementScripts3D.SetActive(false);
            movementScripts2D.SetActive(true);
            
            playerCameraManager.SetRotable(false);
            // playerCameraManager.playerCamera.SetParent(playerCharacterController, true);
        }
        
        private void CastEnd()
        {
            // playerCameraManager.playerCamera.SetParent(_directionMain, true);
            
            _playerCamera.localPosition = _playerCameraDefaultPosition;
            _playerCamera.localRotation = _playerCameraDefaultRotation;
            
            movementScripts3D.SetActive(true);
            movementScripts2D.SetActive(false);
            
            playerCameraManager.SetRotable(true);
            
            _skyView.isInCast = false;
        }
    
        void Start()
        {
            _skyView = spellManager.GetSpellById(2);
            
            _directionMain = playerCameraManager.directionMain;
            _playerCamera = playerCameraManager.playerCamera;
                
            _playerCameraDefaultPosition = _playerCamera.localPosition;
            _playerCameraDefaultRotation = _playerCamera.localRotation;
            
            _skyView.AddSpellListener(_ => SpellCasted());
        }
    
        void FixedUpdate()
        {
            if (!_skyView.isInCast) return;
            
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                CastEnd();
                return;
            }
            
            _playerCamera.localPosition = targetPosition;

            playerCameraManager.SetRotationY(targetEulerRotation.y,_playerCamera);
            playerCameraManager.SetRotationX(targetEulerRotation.x,_playerCamera);
        }
    }
}
