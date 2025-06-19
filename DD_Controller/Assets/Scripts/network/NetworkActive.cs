using System;
using Unity.Netcode;

namespace network
{
    public class NetworkActive : NetworkBehaviour
    {
        [Rpc(SendTo.NotOwner)]
        private void SetActiveRpc(bool active)
        {
            gameObject.SetActive(active);
        }

        private void OnDisable()
        {
            if (NetworkObject.isActiveAndEnabled && IsOwner)
                SetActiveRpc(false);
        }

        private void OnEnable()
        {
            if (NetworkObject.isActiveAndEnabled && IsOwner)
                SetActiveRpc(true);
        }
    }
}