using Globals;
using Unity.Netcode;
using UnityEngine;

namespace initScene
{
    public class LowerScript : MonoBehaviour
    {
        public NetworkObject networkObject;
        public GameObject[] GameObjectsToDestroy;
        public MonoBehaviour[] ComponentsToDestroy;
        public Rigidbody[] RigidbodiesToDestroy;

        public void Start()
        {
            SceneObjectReferencer.WaitingInit += sor =>
            {
                if (sor.isNetworkScene && !networkObject.IsOwner) Apply();
                Destroy(this);
            };

        }

        private void Apply()
        {
            foreach (MonoBehaviour ctd in ComponentsToDestroy)
                Destroy(ctd);

            foreach (GameObject go in GameObjectsToDestroy)
                Destroy(go);

            foreach (Rigidbody rb in RigidbodiesToDestroy)
                Destroy(rb);
        }
    }
}