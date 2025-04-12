using System;
using UnityEngine;

namespace initScene
{
    public class CameraContainer : MonoBehaviour
    {
        public Transform cameraContainer;
        public Vector3 Offset;
        public Vector3 directionOffset;
        public CameraUser[] cameraUsers;
        
        public void DealCamera(Camera camera)
        {
            foreach (CameraUser cameraUser in cameraUsers)
                cameraUser.Camera = camera;
        }
        
    }

    public abstract class CameraUser : MonoBehaviour
    {
        [NonSerialized] public Camera Camera;
    }
}