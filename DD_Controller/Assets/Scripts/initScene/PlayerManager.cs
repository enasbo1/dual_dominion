using Unity.Netcode;
using UnityEngine;

namespace initScene
{
    public class PlayerManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public Transform spawnPoint;
        public NetworkObject networkObject;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            if (!networkObject.IsOwner) return;
            
            var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
            var no = go.GetComponent<NetworkObject>();
            
            if (!no) {
                var ncc = go.GetComponent<NetworkChildContainer>();
                if (ncc)
                    no = ncc.networkObject;
                ncc.GetComponent<Transform>().SetParent(null);
            }
            if (no)
                no.Spawn(true);

            var playerCamera = Camera.main;
            var cc = go.GetComponent<CameraContainer>();
            if (!cc) return;
            if (playerCamera == null) return;
            playerCamera.transform.SetParent(cc.cameraContainer);
            playerCamera.transform.localPosition = cc.Offset;
            playerCamera.transform.rotation = Quaternion.identity;
        }
    }
}
