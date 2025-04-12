using UnityEngine;

namespace script.network
{
    public abstract class NetworkDataOutput<T> : MonoBehaviour
    {
        public abstract T OutNetworkData();
    }


    public abstract class NetworkDataInput<T> : MonoBehaviour
    {
        public abstract void InNetworkData(T networkData);
    }
}