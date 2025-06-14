using Mage;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace initScene
{
    public class PlayerManager : NetworkBehaviour
    {
        public GameObject mageContainerPrefab;
        public GameObject ManagerBearer;
        public PlayerBearer PlayerBearer;
        public WalkerDealingManager playerDealingManager;
        public GameObject godScenePrefab;
        public Transform spawnPoint;
        public NetworkObject networkObject;
        public Transform playerCamera;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer) 
                ManagerBearer.SetActive(false);
            GameObject selectedPrefab = NetworkManager.Singleton.IsServer ? mageContainerPrefab : godScenePrefab;

            GameObject go = Instantiate(selectedPrefab, spawnPoint.position, spawnPoint.rotation);
            if (NetworkManager.Singleton.IsServer)
            {
                PlayerBearer.mainPlayer = go.GetComponent<PlayerDealer>();
                playerDealingManager.Add(PlayerBearer.mainPlayer);
            }
            
            NetworkObject no = go.GetComponent<NetworkObject>();

            if (!no)
            {
                NetworkChildContainer ncc = go.GetComponent<NetworkChildContainer>();
                if (ncc)
                    no = ncc.networkObject;
                ncc.GetComponent<Transform>().SetParent(null);
            }

            if (no)
                no.Spawn(true);

            CameraContainer cameraContainer = go.GetComponent<CameraContainer>();


            playerCamera.SetParent(cameraContainer.cameraContainer);
            playerCamera.localPosition = cameraContainer.Offset;
            playerCamera.rotation = Quaternion.Euler(cameraContainer.directionOffset);
        }
    }
}