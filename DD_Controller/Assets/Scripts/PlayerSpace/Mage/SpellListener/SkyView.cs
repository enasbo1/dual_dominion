using Move;
using UnityEngine;

namespace PlayerSpace.Mage.SpellListener
{
    public class SkyView : MonoBehaviour
    {
        public SpellManager spellManager;

        [Header("Controllers")] public MageCameraController playerCameraController;

        public MageController mageController;

        [Header("Transforms")] public Transform playerCharacterController;

        public Transform mageCharacterTransform;
        public Transform playerCamera;

        [Header("Parameters")] public Vector3 targetPosition = new Vector3(0, 40, -4);

        public Vector3 targetEulerRotation = new Vector3(85, 0, 0);
        private MoveMode _defaultMoveMode;
        private Transform _directionMain;
        private Spell _endSkyView;

        private Vector3 _playerCameraDefaultPosition;
        private Quaternion _playerCameraDefaultRotation;

        private Spell _skyView;
        private float _timeLimit;

        private float _timer;

        private void Start()
        {
            _skyView = spellManager.GetSpellById(2);
            _endSkyView = spellManager.GetSpellById(3);

            _directionMain = playerCameraController.directionMain;
            _defaultMoveMode = mageController.moveMode;
            _playerCameraDefaultPosition = playerCamera.localPosition;
            _playerCameraDefaultRotation = playerCamera.localRotation;

            _skyView.AddSpellListener(_ => SpellCasted());
            _endSkyView.AddSpellListener(_ => _timer = 0);
        }

        private void FixedUpdate()
        {
            if (!_skyView.isInCast) return;

            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                CastEnd();
                return;
            }

            playerCamera.position = mageCharacterTransform.position + targetPosition;

            MageCameraController.SetRotationY(targetEulerRotation.y, playerCamera);
            MageCameraController.SetRotationX(targetEulerRotation.x, playerCamera);
        }

        private void SpellCasted()
        {
            _skyView.isInCast = true;
            _endSkyView.isUnlocked = true;

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
            _endSkyView.isUnlocked = false;
        }
    }
}