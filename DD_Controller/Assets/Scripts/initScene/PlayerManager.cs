using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace initScene
{
    public class PlayerManager : NetworkBehaviour
    {
        public GameObject mageContainerPrefab;
        public GameObject godScenePrefab;
        public Transform spawnPoint;
        public NetworkObject networkObject;
        public Transform playerCamera;

        private void Start()
        {
            GameObject selectedPrefab = NetworkManager.Singleton.IsServer ? mageContainerPrefab : godScenePrefab;

            var go = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            var no = go.GetComponent<NetworkObject>();

            if (!no)
            {
                var ncc = go.GetComponent<NetworkChildContainer>();
                if (ncc)
                    no = ncc.networkObject;
                ncc.GetComponent<Transform>().SetParent(null);
            }

            if (no)
                no.Spawn(true);

            var cameraContainer = go.GetComponent<CameraContainer>();

            playerCamera.SetParent(cameraContainer.cameraContainer);
            playerCamera.localPosition = cameraContainer.Offset;
            playerCamera.rotation = Quaternion.identity;
        }
    }
}
