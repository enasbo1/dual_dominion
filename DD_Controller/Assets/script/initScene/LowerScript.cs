using Unity.Netcode;
using UnityEngine;

namespace script.initScene
{
    public class LowerScript : MonoBehaviour{
        public NetworkObject networkObject;
        public GameObject[] GameObjectsToDestroy;
        public MonoBehaviour[] ComponentsToDestroy;
        public Rigidbody[] RigidbodiesToDestroy;

        public void Start()
        {
            if (!networkObject.IsOwner)
            {
                Apply();
            }
        }

        private void Apply()
        {
            foreach (var ctd in ComponentsToDestroy)
                Destroy(ctd);
            
            foreach (var go in GameObjectsToDestroy)
                Destroy(go);

            foreach (var rb in RigidbodiesToDestroy)
                Destroy(rb);
            Destroy(this);
        }
    }
}
