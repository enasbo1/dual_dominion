using System;
using Globals;
using Unity.Netcode;
using UnityEngine;

namespace network
{
    public class NetworkRenderMaterials : NetworkBehaviour
    {
        [SerializeField] private Renderer[] meshRenderers;

        private Material[] _material = Array.Empty<Material>();
        
        [Rpc(SendTo.NotOwner)]
        void SwitchMaterialRpc(int index, int materialId)
        {
            meshRenderers[index].material = NetworkConverter.GetMaterial(materialId);
        }
        
        // Update is called once per frame
        void FixedUpdate()
        {
            if (!NetworkObject.isActiveAndEnabled) return;
            if (!NetworkObject.IsOwner) return;
            if (_material.Length != meshRenderers.Length)
                _material = new Material[meshRenderers.Length];
            for (int i = 0; i < meshRenderers.Length; i++)
            {
                if (meshRenderers[i].material.Equals(_material[i])) continue;
                _material[i]  = meshRenderers[i].material;
                SwitchMaterialRpc(i, NetworkConverter.GetMaterialId(_material[i]));

            }
            
        }
    }
}
