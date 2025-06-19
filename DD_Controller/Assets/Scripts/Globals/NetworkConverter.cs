using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Globals
{
    public class NetworkConverter : MonoBehaviour
    {
        [SerializeField] private Material[] materialNetworked;
        
        private static NetworkConverter _instance;

        private void Awake()
        {
            _instance = this;
        }
        
        public static int GetMaterialId(Material mat)
        {
            if (!_instance)
            {
                Debug.LogWarning("NetworkConverter has not been yet intitalized");
                return 0;
            }
            for (int i = 0; i < _instance.materialNetworked.Length; i++)
            {
                if (mat.color.Equals(_instance.materialNetworked[i].color)) return i;
            }
            Debug.LogWarning($"Material {mat} not referenced");
            return 0;
        }
        
        public static Material GetMaterial(int materialId)
        {
            if (!_instance)
            {
                Debug.LogWarning("NetworkConverter has not been yet intitalized");
                return new Material(Shader.Find("Standard"));
            }
            if (materialId<_instance.materialNetworked.Length)
                return _instance.materialNetworked[materialId];
            
            Debug.LogWarning("MaterialId out of range");
            return _instance.materialNetworked[0];
        }

        private void OnDestroy()
        {
            _instance = null;
        }
    }
}