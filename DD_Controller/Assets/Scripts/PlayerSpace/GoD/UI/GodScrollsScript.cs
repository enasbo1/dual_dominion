using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSpace.GoD.UI
{
    public class GodScrollsScript : MonoBehaviour
    {
        [SerializeField] private RectTransform scroller;
        
        private RectTransform _viewport;
        private RectTransform _elementsDisplay;
        private readonly List<Button> _elements = new List<Button>();
        
        private Button _buttonUp;
        private Button _buttonDown;
        private Scrollbar _scrollbarVertical;
        
        private Button _buttonLeft;
        private Button _buttonRight;
        private Scrollbar _scrollbarHorizontal;

        private Vector2 _elementDisplayStartPosition;
        private Vector2 _elementDisplayEndPosition;
        const int ELEMENT_DISPLAY_CAPACITY = 4;
        private Vector2 _elementSize;
        private readonly Vector2 _buttonsGap = new Vector2(5f, 5f);

        void MoveUpWithButton()
        {
            Vector2 newPosition = _elementsDisplay.anchoredPosition;
            newPosition.y -= _elementSize.y + _buttonsGap.y;
            
            if (newPosition.y < _elementDisplayStartPosition.y) return;
            _elementsDisplay.anchoredPosition = newPosition;
        }
        
        void MoveDownWithButton()
        {
            Vector2 newPosition = _elementsDisplay.anchoredPosition;
            newPosition.y += _elementSize.y + _buttonsGap.y;
            
            if (newPosition.y > _elementDisplayEndPosition.y) return;
            _elementsDisplay.anchoredPosition = newPosition;
        }
        
        void MoveLeftWithButton()
        {
            Vector2 newPosition = _elementsDisplay.anchoredPosition;
            newPosition.x += _elementSize.x + _buttonsGap.x;
            
            if (newPosition.x > _elementDisplayStartPosition.x) return;
            _elementsDisplay.anchoredPosition = newPosition;
        }
        
        void MoveRightWithButton()
        {
            Vector2 newPosition = _elementsDisplay.anchoredPosition;
            newPosition.x -= _elementSize.x + _buttonsGap.x;
            
            if (newPosition.x < _elementDisplayEndPosition.x) return;
            _elementsDisplay.anchoredPosition = newPosition;
        }
        
        void Awake()
        {
            _viewport = scroller.GetChild(0).GetComponent<RectTransform>();
            _elementsDisplay = _viewport.GetChild(0).GetComponent<RectTransform>();
            
            _buttonUp = scroller.GetChild(1).GetComponent<Button>();
            _buttonDown = scroller.GetChild(2).GetComponent<Button>();
            _scrollbarVertical = scroller.GetChild(3).GetComponent<Scrollbar>();
            
            _buttonLeft = scroller.GetChild(4).GetComponent<Button>();
            _buttonRight = scroller.GetChild(5).GetComponent<Button>();
            _scrollbarHorizontal = scroller.GetChild(6).GetComponent<Scrollbar>();

            foreach (Transform element in _elementsDisplay.transform)
            {
                _elements.Add(element.GetComponent<Button>());
            }

            _elementDisplayStartPosition = _elementsDisplay.anchoredPosition;
            _elementSize = _elements[0].GetComponent<RectTransform>().sizeDelta;
            
            _buttonUp.onClick.AddListener(MoveUpWithButton);
            _buttonDown.onClick.AddListener(MoveDownWithButton);
            _buttonLeft.onClick.AddListener(MoveLeftWithButton);
            _buttonRight.onClick.AddListener(MoveRightWithButton);
        }
        
        void Update()
        {
            _elementDisplayEndPosition = (_elementSize + _buttonsGap) * (_elements.Count - ELEMENT_DISPLAY_CAPACITY) - _elementsDisplay.sizeDelta;
        }
    }
}
