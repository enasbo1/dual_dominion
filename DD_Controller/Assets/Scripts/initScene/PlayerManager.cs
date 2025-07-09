using GoD;
using Mage;
using Monster;
using Unity.Netcode;
using UnityEngine;

namespace initScene
{
    public class PlayerManager : NetworkBehaviour
    {
        public GameObject mageContainerPrefab;
        public GameObject managerBearer;
        public PlayerBearer playerBearer;
        public WalkerDealingManager playerDealingManager;
        public GameObject godScenePrefab;
        public MonsterSpawnScript monsterSpawnScript;
        public Transform spawnPoint;
        public NetworkObject networkObject;
        public Transform playerCamera;

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer) 
                managerBearer.SetActive(false);
            
            
            GameObject selectedPrefab = NetworkManager.Singleton.IsServer ? mageContainerPrefab : godScenePrefab;
            
            GameObject go = Instantiate(selectedPrefab, 
                spawnPoint.position + (NetworkManager.Singleton.IsServer? Vector3.zero : Vector3.up * 30), 
                spawnPoint.rotation);
            if (NetworkManager.Singleton.IsServer)
            {
                PlayerDealer player = go.GetComponent<PlayerDealer>();
                if (!player)
                    Debug.LogWarning("Player dealer est null");
                
                playerBearer.MainPlayer = player;
                playerDealingManager.ForceStart();      
                playerDealingManager.Add(playerBearer.MainPlayer);
            }
            else
            {
                GodManagerScript godManager = go.GetComponentInChildren<GodManagerScript>();
                Transform spawners = go.transform.GetChild(5);
                monsterSpawnScript.spawnLocation = spawners;
                godManager.monsterSpawnScript = monsterSpawnScript;
                monsterSpawnScript.godManagerScript = godManager;
            }
            
            NetworkObject no = go.GetComponent<NetworkObject>();

            if (!no)
            {
                NetworkChildContainer ncc = go.GetComponent<NetworkChildContainer>();
                if (ncc)
                {
                    no = ncc.networkObject;
                    ncc.GetComponent<Transform>().SetParent(null);
                }
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