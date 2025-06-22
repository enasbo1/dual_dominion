using Globals;
using Unity.Netcode;
using UnityEngine;

namespace network
{
    public class NetworkActive : NetworkBehaviour
    {
        [SerializeField] private bool isEnableOnStart = true;

        void Start()
        {
            SceneObjectReferencer.WaitingInit += sor =>
            {
                if (!isEnableOnStart && sor.isNetworkScene && !IsServer)
                    gameObject.SetActive(false);
            };
        }
        
        [Rpc(SendTo.NotServer)]
        private void SetActiveRpc(bool active)
        {
            Debug.Log("here");
            gameObject.SetActive(active);
        }

        private void OnDisable()
        {
            if (SceneObjectReferencer.MainInstance.isNetworkScene && IsServer)
                SetActiveRpc(false);
        }

        private void OnEnable()
        {
            if (SceneObjectReferencer.MainInstance.isNetworkScene && IsServer)
                SetActiveRpc(true);
        }
    }
}
