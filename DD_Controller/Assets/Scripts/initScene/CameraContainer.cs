using Globals;
using UnityEngine;

namespace initScene
{
    public class CameraContainer : MonoBehaviour
    {
        public Transform cameraContainer;
        public Vector3 Offset;
        public Vector3 directionOffset;

        private void Start()
        {
            SceneObjectReferencer.WaitingInit += CamInit;
        }

        private void CamInit(SceneObjectReferencer sceneObjectReferencer)
        {
            Transform camTransform = sceneObjectReferencer.camera.transform;
            camTransform.SetParent(cameraContainer);
            camTransform.localPosition = Offset;
            camTransform.rotation = Quaternion.Euler(directionOffset);
        }
    }
}