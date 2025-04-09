using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mage.SpellListener
{
    public class Grimoire : MonoBehaviour
    {
        public PlayerInput player;
        public SpellManager spellManager;
        [FormerlySerializedAs("grimoire")] public RectTransform grimoireUI;
        [Range(0.1f, 3f)]
        public float transitionSpeed = 0.5f;
        public float scrollFreezeTime = 1.75f;

        private float _timer;
        private float _timeLimit;

        private Spell _grimoireSpell;
        private List<Spell> _spellsAvailable = new List<Spell>();

        private RectTransform _spellsDisplayUI;
        private RectTransform _spellsUI;
        private Vector2 _spellsUIStartPosition;
        private Vector2 _spellsUIPostIncantingPosition;
        private Vector2 _spellsUIEndPosition;

        private Vector2 _spellScroll;
        private Vector2 _grimoireUISizeScroll;
        
        private bool _wasIncanting;
        
        private const float GRIMOIRE_UI_MIN_HEIGHT = 95f;
        private const float GRIMOIRE_UI_MAX_HEIGHT = 200f;
        private const float SPELL_UI_HEIGHT = 70f;

        private readonly Dictionary<int, RectTransform> _spellsForSpellsUI = new Dictionary<int, RectTransform>();
        
        private static void InputDisplay(Graphic img, string hexColor, float rotationAngle)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color)) return;
            
            img.color = color;
            img.rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
        
        private static void SetInputs(List<Image> inputs, Spell spell)
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

        private void SetAllSpellsInUI(List<Spell> spellList)
        {
            int i = 0;
            foreach (Transform spellUI in _spellsUI.transform)
            {
                RectTransform rectSpellUI = spellUI.GetComponent<RectTransform>();
                
                if (i < spellList.Count)
                {
                    Spell spellForThisUI = spellList[i];
                
                    // TODO Add spell icon
                    
                    Transform spellName = spellUI.Find("SpellName");
                    TextMeshProUGUI spellNameUI = spellName.GetComponent<TextMeshProUGUI>();
                    spellNameUI.text = spellForThisUI.name;
                
                    Transform spellInputsUI = spellUI.Find("SpellInputs");
                    List<Image> inputsUI = (from Transform inputUI in spellInputsUI.transform select inputUI.GetComponent<Image>()).ToList();

                    SetInputs(inputsUI, spellForThisUI);
                    
                    _spellsForSpellsUI.Add(spellForThisUI.id, rectSpellUI);
                    i++;
                }
                else spellUI.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            _grimoireSpell = spellManager.GetSpellById(0);
            
            _grimoireUISizeScroll = grimoireUI.sizeDelta;
            _spellsDisplayUI = grimoireUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;

            SetAllSpellsInUI(spellManager.GetSpells());
            _spellsAvailable = spellManager.spellsAvailable;

            _grimoireSpell.AddSpellListener(_ => SpellCasted());
        }
        
        
        private void SetSpellUIPosition(RectTransform spellUI, int spellIndex)
        {
            spellUI.gameObject.SetActive(true);
            Vector2 spellUINewPosition = spellUI.anchoredPosition;
            spellUINewPosition.y = -2.5f - spellIndex * SPELL_UI_HEIGHT;
            spellUI.anchoredPosition = spellUINewPosition;
        }
        
        private void RefreshSpellsUI()
        {
            Spell spellToCast = spellManager.spellToCast;
            RectTransform spellToCastUI = _spellsForSpellsUI[spellToCast.id];
            
            int i = 1;
            foreach (KeyValuePair<int,RectTransform> spellForSpellUI in _spellsForSpellsUI)
            {
                Spell spell = _spellsAvailable.Find(x => x.id == spellForSpellUI.Key);
                RectTransform spellUI = spellForSpellUI.Value;

                if (spell != null && spell.id == spellToCast.id) continue;
                
                if (spell != null)
                {
                    SetSpellUIPosition(spellUI, i);
                    
                    i++;
                }
                else spellUI.gameObject.SetActive(false);
            }
            
            SetSpellUIPosition(spellToCastUI, 0);
        }
        
        private void Update()
        {
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsAvailable.Count - 2) * 70);
            RefreshSpellsUI();
        }

        private void SpellCasted()
        {
            _grimoireSpell.isInCast = true;
            
            _timeLimit = 3 + scrollFreezeTime + SPELL_UI_HEIGHT * _spellsAvailable.Count * Time.fixedDeltaTime / transitionSpeed;
            Debug.Log(_timeLimit);
            _timer = 0;
        }

        private void CastEnd()
        {
            if (spellManager.isIncanting) return;
            
            if (_spellsUI.anchoredPosition.y > _spellsUIStartPosition.y)
            {
                _spellScroll = _spellsUI.anchoredPosition - new Vector2(0f, transitionSpeed);
                _spellsUI.anchoredPosition = _spellScroll;
                return;
            }

            if (grimoireUI.sizeDelta.y > GRIMOIRE_UI_MIN_HEIGHT)
            {
                _grimoireUISizeScroll.y -= transitionSpeed;
                grimoireUI.sizeDelta = _grimoireUISizeScroll;
                _spellsDisplayUI.sizeDelta = _grimoireUISizeScroll - new Vector2(0, 20);
                return;
            }
            
            _grimoireSpell.isInCast = false;
        }

        private void OnIncantingChange()
        {
            if (spellManager.isIncanting)
            {
                _spellsUIPostIncantingPosition = _spellsUI.anchoredPosition;
                _wasIncanting = true;
            }
            else
            {
                _spellsUI.anchoredPosition = _spellsUIPostIncantingPosition;
                _wasIncanting = false;
            }
        }
        
        
        private void FixedUpdate()
        {
            if (!_grimoireSpell.isInCast) return;
            if (spellManager.isIncanting != _wasIncanting) OnIncantingChange();
            
            if (_spellsUI.anchoredPosition.y > _spellsUIEndPosition.y + SPELL_UI_HEIGHT)
            {
                _spellScroll = _spellsUI.anchoredPosition - new Vector2(0f, SPELL_UI_HEIGHT / 10);
                _spellsUI.anchoredPosition = _spellScroll;
            }
            
            if (_timer > _timeLimit)
            {
                CastEnd();
                return;
            }
            
            if (grimoireUI.sizeDelta.y < GRIMOIRE_UI_MAX_HEIGHT)
            {
                _grimoireUISizeScroll.y += transitionSpeed;
                grimoireUI.sizeDelta = _grimoireUISizeScroll;
                _spellsDisplayUI.sizeDelta = _grimoireUISizeScroll - new Vector2(0, 20);
            }
            
            if (spellManager.isIncanting) return;
            
            _timer += Time.deltaTime;

            if (_spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y || _timer < scrollFreezeTime) return;
            
            _spellScroll = _spellsUI.anchoredPosition + new Vector2(0f, transitionSpeed);
            _spellsUI.anchoredPosition = _spellScroll;
        }
    }
}
