using UnityEngine;

namespace Shared
{
    public abstract class Manager : MonoBehaviour
    {
        public abstract void AddElement(ComponentDdDealer element);
        public abstract void DisableElement(ComponentDdDealer element);
    }
}