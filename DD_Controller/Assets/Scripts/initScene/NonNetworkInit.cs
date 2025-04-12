using UnityEngine;

namespace initScene
{
    public class NonNetworkInit : MonoBehaviour
    {
        public CameraContainer cameraContainer;
        public Transform playerCamera;

        private void Start()
        {
            playerCamera.SetParent(cameraContainer.cameraContainer);
            playerCamera.localPosition = cameraContainer.Offset;
            playerCamera.rotation = Quaternion.Euler(cameraContainer.directionOffset);

            cameraContainer.DealCamera(playerCamera.GetComponent<Camera>());
        }
    }
}