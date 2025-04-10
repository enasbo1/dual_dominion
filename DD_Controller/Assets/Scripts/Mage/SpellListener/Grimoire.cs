using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Mage.SpellListener
{
    public class SpellUI
    {
        public Image spellIcon;
        public TextMeshProUGUI spellName;
        public RectTransform spellPosition;
        public List<Image> spellInputs;
        public Image spellStatus;
    }
    
    
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

        private readonly Dictionary<int, SpellUI> _spellsForSpellsUI = new Dictionary<int, SpellUI>();
        
        private static void InputDisplay(Graphic img, string hexColor, float rotationAngle)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color)) return;
            
            img.color = color;
            img.rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }
        
        private static void RefreshInputs(List<Image> inputsUI, Spell spell)
        {
            int i = 0;
            inputsUI.ForEach(inputUI =>
            {
                if (i < spell.inputs.Count)
                {
                    inputUI.gameObject.SetActive(true);
                    switch (spell.inputs[i])
                    {
                        case SpellDirections.Up:
                            InputDisplay(inputsUI[i], spell.canBeCast ? "#FFB600" : "#6E654E", 0f);
                            break;
                        case SpellDirections.Down:
                            InputDisplay(inputsUI[i], spell.canBeCast ? "#009DFF" : "#4D626F", 180f);
                            break;
                        case SpellDirections.Left:
                            InputDisplay(inputsUI[i], spell.canBeCast ? "#00FF15" : "#516B53", 90f);
                            break;
                        case SpellDirections.Right:
                            InputDisplay(inputsUI[i], spell.canBeCast ? "#FF0080" : "#6B545F", -90f);
                            break;
                        case SpellDirections.None:
                        default:
                            break;
                    }
                    
                    i++;
                }
                else inputUI.gameObject.SetActive(false);
            });
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
                    
                    // Get spellUI name
                    Transform spellNameObject = spellUI.Find("SpellName");
                    TextMeshProUGUI spellNameUI = spellNameObject.GetComponent<TextMeshProUGUI>();
                
                    // Get spellUI input images
                    Transform spellInputsObject = spellUI.Find("SpellInputs");
                    List<Image> spellInputsUI = (from Transform inputUI in spellInputsObject.transform select inputUI.GetComponent<Image>()).ToList();
                    
                    // Get spellUI status
                    Transform spellStateObject = spellUI.Find("SpellState");
                    Image spellStateUI = spellStateObject.GetComponent<Image>();
                    
                    // Set UI for the spell
                    spellNameUI.text = spellForThisUI.name;
                    RefreshInputs(spellInputsUI, spellForThisUI);

                    _spellsForSpellsUI.Add(spellForThisUI.id, new SpellUI
                    {
                        spellName = spellNameUI,
                        spellInputs = spellInputsUI,
                        spellPosition = rectSpellUI,
                        spellStatus = spellStateUI
                    });
                    i++;
                }
                else spellUI.gameObject.SetActive(false);
            }
        }

        private void Awake()
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
        
        private void RefreshSpellUIPosition(RectTransform spellUI, int spellIndex)
        {
            spellUI.gameObject.SetActive(true);
            Vector2 spellUINewPosition = spellUI.anchoredPosition;
            spellUINewPosition.y = -2.5f - spellIndex * SPELL_UI_HEIGHT;
            spellUI.anchoredPosition = spellUINewPosition;
        }
        
        private void RefreshSpellUIStatus(Image spellStatus, Spell spell)
        {
            if (spell.canBeCast)
            {
                if (spell.isInCast)
                {
                    spellStatus.gameObject.SetActive(true);
                    
                    string inCastHexColor = "#FFFFFF";
                    if (!ColorUtility.TryParseHtmlString(inCastHexColor, out Color inCastColor)) return;
                    spellStatus.color = inCastColor;
                    return;
                }
                
                spellStatus.gameObject.SetActive(false);
                return;
            }

            spellStatus.gameObject.SetActive(true);

            string cooldownHexColor = "#4D4D4D";
            if (!ColorUtility.TryParseHtmlString(cooldownHexColor, out Color cooldownColor)) return;
            spellStatus.color = cooldownColor;
            
            spellStatus.fillAmount = spell.recastDelay > 0f ? Mathf.Clamp01(1f - (spell.cooldown / spell.recastDelay)) : 1f;
        }
        
        private void RefreshSpellNameUI(SpellUI spellUI, Spell spell)
        {
            string newNameColor = spell.canBeCast ? "#FFFFFF" : "#888888";
            if (!ColorUtility.TryParseHtmlString(newNameColor, out Color color)) return;
            spellUI.spellName.color = color;
        }
        
        private void RefreshSpellsUI()
        {
            Spell spellToCast = spellManager.spellToCast;
            SpellUI spellToCastUI = _spellsForSpellsUI[spellToCast.id];
            int spellToCastId = spellToCast.id;
            
            int i = 1;
            foreach (KeyValuePair<int, SpellUI> spellForSpellUI in _spellsForSpellsUI)
            {
                int spellId = spellForSpellUI.Key;
                SpellUI spellUI = spellForSpellUI.Value;
                
                if (_spellsAvailable.All(s => s.id != spellId)) {
                    spellUI.spellPosition.gameObject.SetActive(false);
                    continue;
                }

                if (spellId == spellToCastId) continue;
                
                Spell spell = _spellsAvailable.First(s => s.id == spellId);
                
                RefreshSpellUIPosition(spellUI.spellPosition, i);
                
                RefreshSpellNameUI(spellUI, spell);
                RefreshSpellUIStatus(spellUI.spellStatus, spell);
                RefreshInputs(spellUI.spellInputs, spell);
                    
                i++;
            }
            
            RefreshSpellUIPosition(spellToCastUI.spellPosition, 0);
            
            RefreshSpellNameUI(spellToCastUI, spellToCast);
            RefreshSpellUIStatus(spellToCastUI.spellStatus, spellToCast);
            RefreshInputs(spellToCastUI.spellInputs, spellToCast);
        }
        
        private void Update()
        {
            RefreshSpellsUI();
            if (!_grimoireSpell.isInCast) return;
            
            _timeLimit = 3 + scrollFreezeTime + SPELL_UI_HEIGHT * _spellsAvailable.Count * Time.fixedDeltaTime / transitionSpeed;
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsAvailable.Count - 2) * 70);
        }

        private void SpellCasted()
        {
            _grimoireSpell.isInCast = true;
            _timer = 0;
        }

        private void CastEnd()
        {
            if (spellManager.isIncanting) return;
            
            // Scroll back spells
            if (_spellsUI.anchoredPosition.y > _spellsUIStartPosition.y)
            {
                _spellScroll = _spellsUI.anchoredPosition - new Vector2(0f, transitionSpeed);
                _spellsUI.anchoredPosition = _spellScroll;
                return;
            }

            // Close Grimory UI
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
            
            // Scroll back spells so that the 1st spell is on top of the UI
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
            
            // Open Grimory UI
            if (grimoireUI.sizeDelta.y < GRIMOIRE_UI_MAX_HEIGHT)
            {
                _grimoireUISizeScroll.y += transitionSpeed;
                grimoireUI.sizeDelta = _grimoireUISizeScroll;
                _spellsDisplayUI.sizeDelta = _grimoireUISizeScroll - new Vector2(0, 20);
            }
            
            if (spellManager.isIncanting) return;
            
            _timer += Time.deltaTime;

            // Scroll spells
            if (_spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y || _timer < scrollFreezeTime) return;
            _spellScroll = _spellsUI.anchoredPosition + new Vector2(0f, transitionSpeed);
            _spellsUI.anchoredPosition = _spellScroll;
        }
    }
}
