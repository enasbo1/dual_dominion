using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerSpace.Mage.UI
{
    public class SpellUI
    {
        public Image spellIcon;
        public List<Image> spellInputs;
        public TextMeshProUGUI spellName;
        public RectTransform spellPosition;
        public Image spellStatus;
    }
    
    public class SpellUIDisplayState
    {
        public int id;
        public bool isVisible;
        public Vector2 position;
        public Color nameColor;
        public bool showStatus;
        public Color statusColor;
        public float statusFillAmount;
        public SpellDirections[] directions;
    }


    public class LevelUpGrimoire : MonoBehaviour
    {
        private const float SPELL_UI_HEIGHT = 70f;
        private readonly Dictionary<int, SpellUI> _spellsForSpellsUI = new Dictionary<int, SpellUI>();
        
        public MageUIRendererScript mageUIRenderer;
        public SpellManager spellManager;
        public RectTransform levelUpGrimoireUI;

        [Range(0.1f, 5f)] public float transitionDistance = 0.8f;
        
        private float _localDeltaTime;
        private float _localLastFrameTime;

        private Vector2 _scrollSpell;
        private List<Spell> _spellsToUnlock = new List<Spell>();

        private RectTransform _spellsDisplayUI;
        private RectTransform _spellsUI;
        private Vector2 _spellsUIEndPosition;
        private Vector2 _spellsUIPostIncantingPosition;
        private Vector2 _spellsUIStartPosition;
        private float _timeLimit;
        private float _timer;
        
        private float _grimoryOpeningTime;

        private bool _wasIncanting;

        private void Awake()
        {
            _spellsDisplayUI = levelUpGrimoireUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;

            SetAllSpellsInUI(spellManager.GetSpells());
            _spellsToUnlock = spellManager.spellsToUnlock;
        }
        
        private void CastEnd()
        {
            if (spellManager.isIncanting) return;

            // Scroll back spells
            if (_spellsUI.anchoredPosition.y > _spellsUIStartPosition.y)
            {
                _scrollSpell = _spellsUI.anchoredPosition - new Vector2(0f, transitionDistance);
                _spellsUI.anchoredPosition = _scrollSpell;
                return;
            }

            float transitionSpeed = _localDeltaTime / transitionDistance;
            float waitTime = SPELL_UI_HEIGHT * (2 / transitionDistance * transitionSpeed);
            _timer = -waitTime;
        }
        
        private void SpellScroll()
        {
            if (spellManager.isIncanting != _wasIncanting) OnIncantingChange();

            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsToUnlock.Count - 2) * 70);
            
            // Scroll back spells so that the 1st spell is on top of the UI
            if (_spellsUI.anchoredPosition.y > _spellsUIEndPosition.y + SPELL_UI_HEIGHT)
            {
                _scrollSpell = _spellsUI.anchoredPosition - new Vector2(0f, SPELL_UI_HEIGHT / (SPELL_UI_HEIGHT / 5));
                _spellsUI.anchoredPosition = _scrollSpell;
            }

            if (_timer > _timeLimit)
            {
                CastEnd();
                return;
            }

            if (spellManager.isIncanting) return;

            _timer += _localDeltaTime;
            
            if (_timer < 0f) return;
            
            // Scroll spells
            if (_spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y) return;
            _scrollSpell = _spellsUI.anchoredPosition + new Vector2(0f, transitionDistance);
            _spellsUI.anchoredPosition = _scrollSpell;
        }

        private void Update()
        {
            SpellScroll();
            RefreshSpellsUI();
        }

        private void FixedUpdate()
        {
            _localDeltaTime = Time.unscaledTime - _localLastFrameTime;
            _localLastFrameTime = Time.unscaledTime;
            
            if (spellManager.isIncanting) return;
            
            float transitionSpeed = _localDeltaTime / transitionDistance;
            _timeLimit = SPELL_UI_HEIGHT * ((_spellsToUnlock.Count + 1 / transitionDistance) * transitionSpeed);
        }

        private static void InputDisplay(Graphic img, Color color, float rotationAngle)
        {
            img.color = color;
            img.rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }

        private void RefreshInputs(List<Image> inputsUI, Spell spell)
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
                            InputDisplay(inputsUI[i],
                                mageUIRenderer.upArrowColor,
                                0f);
                            break;
                        case SpellDirections.Right:
                            InputDisplay(inputsUI[i],
                                mageUIRenderer.rightArrowColor,
                                -90f);
                            break;
                        case SpellDirections.Down:
                            InputDisplay(inputsUI[i],
                                mageUIRenderer.downArrowColor,
                                180f);
                            break;
                        case SpellDirections.Left:
                            InputDisplay(inputsUI[i],
                                mageUIRenderer.leftArrowColor,
                                90f);
                            break;
                        case SpellDirections.None:
                        default:
                            break;
                    }

                    i++;
                }
                else
                {
                    inputUI.gameObject.SetActive(false);
                }
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
                    spellNameUI.color = mageUIRenderer.spellNameColorOnCast;

                    // Get spellUI input images
                    Transform spellInputsObject = spellUI.Find("SpellInputs");
                    List<Image> spellInputsUI = (from Transform inputUI in spellInputsObject.transform
                        select inputUI.GetComponent<Image>()).ToList();

                    // Get spellUI status
                    Transform spellStateObject = spellUI.Find("SpellState");
                    Image spellStateUI = spellStateObject.GetComponent<Image>();
                    spellStateUI.gameObject.SetActive(false);

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
                else
                {
                    spellUI.gameObject.SetActive(false);
                }
            }
        }
        
        private List<SpellUIDisplayState> ComputeSpellUIStates()
        {
            List<SpellUIDisplayState> displayStates = new List<SpellUIDisplayState>();
            Spell spellToCast = spellManager.spellToCast;
            int spellToCastId = spellToCast.id;

            Vector2 startPos = _spellsUIStartPosition;
            float yOffset = -2.5f;
            int index = 1;

            foreach (KeyValuePair<int, SpellUI> spellForSpellUI in _spellsForSpellsUI)
            {
                int spellIdForThisUI = spellForSpellUI.Key;
                bool isUnlockable = _spellsToUnlock.Exists(s => s.id == spellIdForThisUI);

                if (!isUnlockable)
                {
                    displayStates.Add(new SpellUIDisplayState
                    {
                        id = spellIdForThisUI,
                        isVisible = false
                    });
                    continue;
                }
                
                Spell spell = _spellsToUnlock.First(s => s.id == spellIdForThisUI);
                if (spellIdForThisUI == spellToCastId) continue;
                
                if (spell.isHidden)
                {
                    displayStates.Add(new SpellUIDisplayState
                    {
                        id = spellIdForThisUI,
                        isVisible = false
                    });
                    continue;
                }
                
                SpellUIDisplayState state = new SpellUIDisplayState
                {
                    id = spellIdForThisUI,
                    isVisible = !spell.isHidden || spellToCast.id == spellIdForThisUI,
                    position = new Vector2(startPos.x, yOffset - index * SPELL_UI_HEIGHT),
                    directions = spell.inputs.ToArray()
                };

                displayStates.Add(state);
                index++;
            }

            displayStates.Add(new SpellUIDisplayState
            {
                id = spellToCast.id,
                isVisible = true,
                position = new Vector2(startPos.x, yOffset),
                directions = spellToCast.inputs.ToArray()
            });

            return displayStates;
        }
        
        private void RefreshSpellsUI()
        {
            List<SpellUIDisplayState> displayStates = ComputeSpellUIStates();

            foreach (SpellUIDisplayState state in displayStates)
            {
                if (!_spellsForSpellsUI.TryGetValue(state.id, out SpellUI ui)) continue;

                GameObject spellPositionObject = ui.spellPosition.gameObject;
                
                if (!state.isVisible)
                {
                    if (spellPositionObject.activeSelf)
                        spellPositionObject.SetActive(false);
                    continue;
                }

                // Position
                spellPositionObject.SetActive(true);
                ui.spellPosition.anchoredPosition = state.position;

                // Spell Inputs
                for (int i = 0; i < ui.spellInputs.Count; i++)
                {
                    Image spellInput = ui.spellInputs[i];
                    if (i < state.directions.Length)
                    {
                        spellInput.gameObject.SetActive(true);
                        float angle;
                        Color color;

                        switch (state.directions[i])
                        {
                            case SpellDirections.Up:
                                angle = 0f;
                                color = mageUIRenderer.upArrowColor;
                                break;
                            case SpellDirections.Right:
                                angle = -90f;
                                color = mageUIRenderer.rightArrowColor;
                                break;
                            case SpellDirections.Down:
                                angle = 180f;
                                color = mageUIRenderer.downArrowColor;
                                break;
                            case SpellDirections.Left:
                                angle = 90f;
                                color = mageUIRenderer.leftArrowColor;
                                break;
                            default:
                                continue;
                        }

                        spellInput.color = color;
                        spellInput.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    else
                    {
                        spellInput.gameObject.SetActive(false);
                    }
                }
            }
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
    }
}