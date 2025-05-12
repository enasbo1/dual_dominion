using UnityEngine;
using UnityEngine.EventSystems;

namespace GoD
{
    public class OnScrollRectClickScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public bool isPointerOverScrollRect;

        public void OnPointerDown(PointerEventData eventData)
        {
            isPointerOverScrollRect = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPointerOverScrollRect = false;
        }

        public void Update()
        {
            Debug.Log(isPointerOverScrollRect);
        }
    }
}
