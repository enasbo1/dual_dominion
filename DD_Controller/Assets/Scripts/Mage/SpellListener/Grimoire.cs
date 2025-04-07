using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
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
        public float scrollFreezeTime = 2f;

        private float _timer;
        private float _timeLimit;

        private Spell _grimoireSpell;
        private List<Spell> _spellList;

        private RectTransform _spellsDisplayUI;
        private RectTransform _spellsUI;
        private Vector2 _spellsUIStartPosition;
        private Vector2 _spellsUIEndPosition;

        private Vector2 _spellScroll;
        private Vector2 _grimoireUISizeScroll; // Used for width and height of grimoireUI and spellsUI
        
        private bool _isIncanting;
        
        private InputAction _incantationTrigger;
        private InputAction _spellTrigger;
        
        private const float GRIMOIRE_UI_MIN_HEIGHT = 95f;
        private const float GRIMOIRE_UI_MAX_HEIGHT = 200f;
        private const float SPELL_UI_HEIGHT = 70f;
        
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

        private void RefreshSpellsUI()
        {
            int i = 0;
            foreach (Transform spellUI in _spellsUI.transform)
            {
                RectTransform spellUIPosition = spellUI.GetComponent<RectTransform>();

                Vector2 spellUINewPosition = spellUIPosition.anchoredPosition;
                spellUINewPosition.y = -2.5f - i * SPELL_UI_HEIGHT;
                spellUIPosition.anchoredPosition = spellUINewPosition;
                
                if (i < _spellList.Count)
                {
                    Spell spellForThisUI = _spellList[i];
                
                    Transform spellName = spellUI.Find("SpellName");
                    TextMeshProUGUI spellNameUI = spellName.GetComponent<TextMeshProUGUI>();
                    spellNameUI.text = spellForThisUI.name;
                
                    Transform spellInputsUI = spellUI.Find("SpellInputs");
                    List<Image> inputsUI = (from Transform inputUI in spellInputsUI.transform select inputUI.GetComponent<Image>()).ToList();

                    SetInputs(inputsUI, spellForThisUI);
                    
                    i++;
                }
                else spellUI.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            _grimoireSpell = spellManager.GetSpellById(0);
            _spellList = spellManager.GetSpells();
            
            _grimoireUISizeScroll = grimoireUI.sizeDelta;
            _spellsDisplayUI = grimoireUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;

            RefreshSpellsUI();
            
            _incantationTrigger = player.actions["IncantationTrigger"];
            _incantationTrigger.started += _ => _isIncanting = true;
            
            _spellTrigger = player.actions["CastSpell"];
            _spellTrigger.started += _ => _isIncanting = false;
            
            _grimoireSpell.AddSpellListener(_ => SpellCasted());
            
            CastEnd();
        }

        private void SpellCasted()
        {
            _grimoireSpell.isInCast = true;
            
            _timeLimit = 4 + scrollFreezeTime + 3 * (_spellList.Count - 2) / (17 *  math.pow(transitionSpeed, 2.8f));
            Debug.Log(_timeLimit);
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellList.Count - 2) * 70);
            
            _timer = _timeLimit;
            
        }

        private void CastEnd()
        {
            if (_isIncanting) return;
            
            if (_spellsUI.anchoredPosition.y > _spellsUIStartPosition.y)
            {
                _spellScroll = _spellsUI.anchoredPosition - new Vector2(0f, transitionSpeed);
                _spellsUI.anchoredPosition = _spellScroll;
                return;
            }

            if (grimoireUI.sizeDelta.y <= GRIMOIRE_UI_MIN_HEIGHT) return;
            
            _grimoireUISizeScroll.y -= transitionSpeed;
            grimoireUI.sizeDelta = _grimoireUISizeScroll;
            _spellsDisplayUI.sizeDelta = _grimoireUISizeScroll - new Vector2(0, 20);
            
            _grimoireSpell.isInCast = false;
        }

        private void FixedUpdate()
        {
            if (_timer <= 0 && grimoireUI.sizeDelta.y >= GRIMOIRE_UI_MIN_HEIGHT) CastEnd();
            if (_timer <= 0) return;
            
            if (grimoireUI.sizeDelta.y < GRIMOIRE_UI_MAX_HEIGHT)
            {
                _grimoireUISizeScroll.y += transitionSpeed;
                grimoireUI.sizeDelta = _grimoireUISizeScroll;
                _spellsDisplayUI.sizeDelta = _grimoireUISizeScroll - new Vector2(0, 20);
            }

            if (_isIncanting) return;
            
            _timer -= Time.deltaTime;

            if (_spellList.Count <= 2 || _timer > _timeLimit - scrollFreezeTime || _spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y) return;
            _spellScroll = _spellsUI.anchoredPosition + new Vector2(0f, transitionSpeed);
            _spellsUI.anchoredPosition = _spellScroll;
        }
    }
}
