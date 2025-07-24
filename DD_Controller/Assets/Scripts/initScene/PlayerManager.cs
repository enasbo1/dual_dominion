using GoD;
using Menu;
using PlayerSpace.GoD;
using PlayerSpace.Mage;
using Unity.Netcode;
using UnityEngine;

namespace initScene
{
    public class PlayerManager : NetworkBehaviour
    {
        [Header("Prefabs")]
        public GameObject mageContainerPrefab;
        public GameObject godContainerPrefab;
        
        [Header("")]
        public GameObject managerBearer;
        public PlayerBearer playerBearer;
        public WalkerDealingManager playerDealingManager;
        public MonsterSpawnScript monsterSpawnScript;
        public PauseMenuScript pauseMenu;
        public Transform spawnPoint;
        public Transform playerCamera;

        private NetworkManager _networkManager;

        private void Start()
        {
            _networkManager = NetworkManager.Singleton;
            
            if (!_networkManager.IsServer) 
                managerBearer.SetActive(false);
            
            
            GameObject selectedPrefab = NetworkManager.Singleton.IsServer ? mageContainerPrefab : godContainerPrefab;
            
            GameObject go = Instantiate(selectedPrefab, 
                spawnPoint.position + (_networkManager.IsServer? Vector3.zero : Vector3.up * 30), 
                spawnPoint.rotation);
            
            if (_networkManager.IsServer)
            {
                MageDealer player = go.GetComponent<MageDealer>();
                if (!player)
                    Debug.LogWarning("Player dealer est null");
                
                playerBearer.MainPlayer = player;
                playerDealingManager.ForceStart();
                playerDealingManager.Add(playerBearer.MainPlayer);
            }
            else
            {
                GodSelectMonsterManagerScript godSelectMonsterManager = go.GetComponentInChildren<GodSelectMonsterManagerScript>();
                GodManagerScript godManager = go.GetComponent<GodManagerScript>();
                
                godSelectMonsterManager.monsterSpawnScript = monsterSpawnScript;

                monsterSpawnScript.godManager = godManager;
                monsterSpawnScript.godSelectMonsterManagerScript = godSelectMonsterManager;
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