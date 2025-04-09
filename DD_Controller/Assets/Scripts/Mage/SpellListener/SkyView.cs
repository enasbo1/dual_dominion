using Move;
using UnityEngine;

namespace Mage.SpellListener
{
    public class SkyView : MonoBehaviour
    {
        public SpellManager spellManager;
        
        [Header("Controllers")]
        public MageCameraController playerCameraController;
        public MageController mageController;
        
        [Header("Transforms")]
        public Transform playerCharacterController;
        public Transform mageCharacterTransform;
        public Transform playerCamera;
        
        [Header("Parameters")]
        public Vector3 targetPosition = new (0, 40, -4);
        public Vector3 targetEulerRotation = new (85, 0, 0);

        private Spell _skyView;
        
        private float _timer;
        private float _timeLimit;
        private MoveMode _defaultMoveMode;
        private Transform _directionMain;
        
        private Vector3 _playerCameraDefaultPosition;
        private Quaternion _playerCameraDefaultRotation;
    
        private void SpellCasted()
        {
            _skyView.isInCast = true;
            
            _timeLimit = 20f;
            _timer = _timeLimit;
            
            mageController.moveMode = MoveMode.UpView;
            
            playerCameraController.moveMode = MoveMode.UpView;
            playerCamera.SetParent(playerCharacterController, true);
        }
        
        private void CastEnd()
        {
            playerCamera.SetParent(_directionMain, true);
            
            playerCamera.localPosition = _playerCameraDefaultPosition;
            playerCamera.localRotation = _playerCameraDefaultRotation;
            
            mageController.moveMode = _defaultMoveMode;
            
            playerCameraController.moveMode = _defaultMoveMode;
            
            _skyView.isInCast = false;
        }
    
        void Start()
        {
            _skyView = spellManager.GetSpellById(2);
            
            _directionMain = playerCameraController.directionMain;
            _defaultMoveMode = mageController.moveMode;
            _playerCameraDefaultPosition = playerCamera.localPosition;
            _playerCameraDefaultRotation = playerCamera.localRotation;
            
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
            
            playerCamera.position = mageCharacterTransform.position +  targetPosition;

            MageCameraController.SetRotationY(targetEulerRotation.y,playerCamera);
            MageCameraController.SetRotationX(targetEulerRotation.x,playerCamera);
        }
    }
}
