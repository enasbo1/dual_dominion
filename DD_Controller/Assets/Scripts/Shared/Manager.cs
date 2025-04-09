using UnityEngine;

namespace Shared
{
    public abstract class Manager<TDealer> : MonoBehaviour
    {
        public abstract void AddElement(TDealer element);
        public abstract void DisableElement(TDealer element);
    }
}