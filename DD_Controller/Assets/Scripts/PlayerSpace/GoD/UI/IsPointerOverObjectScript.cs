using UnityEngine;
using UnityEngine.EventSystems;

namespace PlayerSpace.GoD.UI
{
    public class IsPointerOverObjectScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public GodPointerManagerScript pointerManager;

        private void Start()
        {
            foreach (Transform element in gameObject.transform)
            {
                if (!element.TryGetComponent<IsPointerOverObjectScript>(out _))
                {
                    IsPointerOverObjectScript newScript = element.gameObject.AddComponent<IsPointerOverObjectScript>();
                    newScript.pointerManager = this.pointerManager;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            pointerManager.isPointerOverScrollRect = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            pointerManager.isPointerOverScrollRect = false;
        }
    }
}
