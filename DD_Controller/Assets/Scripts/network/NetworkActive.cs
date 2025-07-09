using Globals;
using JetBrains.Annotations;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

namespace network
{
    public class NetworkActive : NetworkBehaviour
    {
        [SerializeField] private bool isEnableOnStart = true;

        [CanBeNull] private Rigidbody _rigidbody;
        private bool _hasNetworkTransform;
        void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _hasNetworkTransform = GetComponent<NetworkTransform>();
            SceneObjectReferencer.WaitingInit += sor =>
            {
                if (!isEnableOnStart && sor.isNetworkScene && !IsServer)
                    gameObject.SetActive(false);
            };
        }
        
        [Rpc(SendTo.NotServer)]
        private void SetActiveRpc(bool active, Vector3 position = default, Quaternion rotation = default)
        {
            Debug.Log("here");
            gameObject.SetActive(active);
            if (!active) return;
            if (!_hasNetworkTransform) return;
            if (position != Vector3.zero) transform.position = new Vector3(position.x, position.y, position.z);
            
            if (rotation == Quaternion.identity) return;
            if (_rigidbody) _rigidbody.rotation = rotation;
            else transform.rotation = rotation;
        }

        private void OnDisable()
        {
            if (!SceneObjectReferencer.MainInstance) return;
            if (SceneObjectReferencer.MainInstance.isNetworkScene && IsServer)
                SetActiveRpc(false);
        }

        private void OnEnable()
        {            
            if (!SceneObjectReferencer.MainInstance) return;
            if (SceneObjectReferencer.MainInstance.isNetworkScene && IsServer)
                SetActiveRpc(true, position:transform.position, rotation:transform.rotation);
        }
    }
}
