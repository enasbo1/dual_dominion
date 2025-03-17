using System.Collections.Generic;
using System.Linq;
using Mage;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace script.Mage.SpellListener
{
    public class Grimoire : MonoBehaviour
    {
        public SpellManager spellManager;
        public RectTransform grimoire;
        [Range(0.1f, 3f)]
        public float transitionSpeed = 0.15f;

        private float _timer;
        private List<Spell> _spellList;

        private float _timeLimit;
        private RectTransform _spellsDisplayUI;
        private RectTransform _spellsUI;
        private Vector2 _spellsUIStartPosition;
        private Vector2 _spellsUIEndPosition;
        private Vector2 _sizeDelta; // Used for width and height
        
        private bool _spellCasted;
        
        private void InputDisplay(Image img, string hexColor, float rotationAngle)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color)) return;
            
            img.color = color;
            img.rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
        
        private void SetInputs(List<Image> inputs, Spell spell)
        {
            int i = 0;
            foreach (Image inputUI in inputs)
            {
                if (i < spell.inputs.Count)
                {
                    switch (spell.inputs[i])
                    {
                        case SpellDirections.Up:
                            InputDisplay(inputs[i], "#FFB600", 0f);
                            break;
                        case SpellDirections.Down:
                            InputDisplay(inputs[i], "#009DFF", 180f);
                            break;
                        case SpellDirections.Left:
                            InputDisplay(inputs[i], "#00FF15", 90f);
                            break;
                        case SpellDirections.Right:
                            InputDisplay(inputs[i], "#FF0080", -90f);
                            break;
                        case SpellDirections.None:
                        default:
                            break;
                    }
                    
                    i++;
                }
                else Destroy(inputUI.gameObject);
            }
        }
        
        void Start()
        {
            _spellList = spellManager.GetSpells();
            
            _sizeDelta = grimoire.sizeDelta;
            _spellsDisplayUI = grimoire.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;
            int i = 0;
            foreach (Transform spellUI in _spellsUI.transform)
            {
                if (i < _spellList.Count)
                {
                    Spell spellForThisUI = _spellList[i];
                
                    Transform spellName = spellUI.Find("SpellName");
                    TextMeshProUGUI spellNameUI = spellName.GetComponent<TextMeshProUGUI>();
                    spellNameUI.text = spellForThisUI.name;
                
                    Transform spellInputsUI = spellUI.Find("SpellInputs");
                    List<Image> inputs = (from Transform child in spellInputsUI.transform select child.GetComponent<Image>()).ToList();

                    SetInputs(inputs, spellForThisUI);
                    
                    i++;
                }
                else Destroy(spellUI.gameObject); // May be replaced by setActive(), so that we can increase spell in book during party
            }
            
            spellManager.GetSpellById(0).AddSpellListener(_ => SpellCasted());
            
            CastEnd();
        }

        void SpellCasted()
        {
            _timeLimit = 5 + 3 * (_spellList.Count - 2) / (17 *  math.pow(transitionSpeed, 2.8f));
            Debug.Log(_timeLimit);
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellList.Count - 2) * 70);
            
            _timer = _timeLimit;
            
            _sizeDelta.y = 200;
            grimoire.sizeDelta = _sizeDelta;
            _sizeDelta.y = 180;
            _spellsDisplayUI.sizeDelta = _sizeDelta;
            
            _spellCasted = true;
        }

        void CastEnd()
        {
            _spellCasted = false;
            
            _sizeDelta.y = 95;
            grimoire.sizeDelta = _sizeDelta;
            _sizeDelta.y = 75;
            _spellsDisplayUI.sizeDelta = _sizeDelta;
            _spellsUI.anchoredPosition = _spellsUIStartPosition;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_spellCasted) return;
            
            _timer -= Time.fixedDeltaTime;
            if (_timer <= 0)
            {
                CastEnd();
                return;
            }
            
            if (_spellList.Count > 2 && _timer <= _timeLimit - 1)
            {
                Vector2 inputMovements = _spellsUI.anchoredPosition + new Vector2(0f, transitionSpeed);
                if (inputMovements.y < _spellsUIEndPosition.y) _spellsUI.anchoredPosition = inputMovements;
            }
        }
    }
}
