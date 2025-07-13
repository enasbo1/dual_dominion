using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayerSpace.Mage.UI;

namespace PlayerSpace.Mage.SpellListener
{
    public class Grimoire : MonoBehaviour
    {
        private const float GRIMOIRE_UI_MIN_HEIGHT = 95f;
        private const float GRIMOIRE_UI_MAX_HEIGHT = 200f;
        private const float SPELL_UI_HEIGHT = 70f;
        public MageUIRendererScript mageUIRenderer;
        public SpellManager spellManager;
        public RectTransform grimoireUI;

        [Range(0.1f, 5f)] public float transitionDistance = 1f;

        private readonly Dictionary<int, SpellUI> _spellsForSpellsUI = new Dictionary<int, SpellUI>();

        private Spell _grimoireSpell;
        private float _grimoryOpeningTime;
        private Vector2 _scrollGrimoryUISize;

        private Vector2 _scrollSpell;
        private List<Spell> _spellsAvailable = new List<Spell>();

        private RectTransform _spellsDisplayUI;
        private RectTransform _spellsUI;
        private Vector2 _spellsUIEndPosition;
        private Vector2 _spellsUIPostIncantingPosition;
        private Vector2 _spellsUIStartPosition;
        private float _timeLimit;

        private float _timer;

        private bool _wasIncanting;

        private void Awake()
        {
            _grimoireSpell = spellManager.GetSpellById(0);

            _scrollGrimoryUISize = grimoireUI.sizeDelta;
            _spellsDisplayUI = grimoireUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;

            SetAllSpellsInUI(spellManager.GetSpells());
            _spellsAvailable = spellManager.spellsAvailable;

            _grimoireSpell.AddSpellListener(_ => SpellCasted());
        }

        private void Update()
        {
            RefreshSpellsUI();
            if (!_grimoireSpell.isInCast) return;
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsAvailable.Count - 3) * 70);

            if (spellManager.isIncanting) return;
            float transitionSpeed = Time.fixedDeltaTime / transitionDistance;
            _grimoryOpeningTime = 1.1f * ((GRIMOIRE_UI_MAX_HEIGHT - GRIMOIRE_UI_MIN_HEIGHT) * transitionSpeed);
            _timeLimit = SPELL_UI_HEIGHT * ((_spellsAvailable.Count + 1 / transitionDistance) * transitionSpeed);
        }


        private void FixedUpdate()
        {
            if (!_grimoireSpell.isInCast) return;
            if (spellManager.isIncanting != _wasIncanting) OnIncantingChange();

            // Scroll back spells so that the 1st spell is on top of the UI
            if (_spellsUI.anchoredPosition.y > _spellsUIEndPosition.y + SPELL_UI_HEIGHT * 1.5f)
            {
                _scrollSpell = _spellsUI.anchoredPosition - new Vector2(0f, SPELL_UI_HEIGHT / 10);
                _spellsUI.anchoredPosition = _scrollSpell;
            }

            if (_timer > _timeLimit)
            {
                CastEnd();
                return;
            }

            // Open Grimory UI
            if (grimoireUI.sizeDelta.y < GRIMOIRE_UI_MAX_HEIGHT)
            {
                _scrollGrimoryUISize.y += transitionDistance;
                grimoireUI.sizeDelta = _scrollGrimoryUISize;
                _spellsDisplayUI.sizeDelta = _scrollGrimoryUISize - new Vector2(0, 20);
            }

            if (spellManager.isIncanting) return;

            _timer += Time.deltaTime;

            // Scroll spells
            if (_spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y || _timer < _grimoryOpeningTime) return;
            _scrollSpell = _spellsUI.anchoredPosition + new Vector2(0f, transitionDistance);
            _spellsUI.anchoredPosition = _scrollSpell;
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
                                spell.canBeCast ? mageUIRenderer.upArrowColor : mageUIRenderer.upDisabledArrowColor,
                                0f);
                            break;
                        case SpellDirections.Right:
                            InputDisplay(inputsUI[i],
                                spell.canBeCast
                                    ? mageUIRenderer.rightArrowColor
                                    : mageUIRenderer.rightDisabledArrowColor, -90f);
                            break;
                        case SpellDirections.Down:
                            InputDisplay(inputsUI[i],
                                spell.canBeCast ? mageUIRenderer.downArrowColor : mageUIRenderer.downDisabledArrowColor,
                                180f);
                            break;
                        case SpellDirections.Left:
                            InputDisplay(inputsUI[i],
                                spell.canBeCast ? mageUIRenderer.leftArrowColor : mageUIRenderer.leftDisabledArrowColor,
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

                    // Get spellUI input images
                    Transform spellInputsObject = spellUI.Find("SpellInputs");
                    List<Image> spellInputsUI = (from Transform inputUI in spellInputsObject.transform
                        select inputUI.GetComponent<Image>()).ToList();

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
                bool isAvailable = _spellsAvailable.Exists(s => s.id == spellIdForThisUI);

                if (!isAvailable)
                {
                    displayStates.Add(new SpellUIDisplayState
                    {
                        id = spellIdForThisUI,
                        isVisible = false
                    });
                    continue;
                }
                
                Spell spell = _spellsAvailable.First(s => s.id == spellIdForThisUI);
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
                    nameColor = spell.canBeCast
                        ? mageUIRenderer.spellNameColorOnCast
                        : mageUIRenderer.spellNameColorOnCooldown,
                    showStatus = !spell.canBeCast || spell.isInCast,
                    statusColor = spell.isInCast
                        ? mageUIRenderer.spellBackgroundColorInCast
                        : mageUIRenderer.spellBackgroundColorOnCooldown,
                    statusFillAmount = spell.recastDelay > 0f
                        ? Mathf.Clamp01(1f - spell.cooldown / spell.recastDelay)
                        : 1f,
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
                nameColor = spellToCast.canBeCast
                    ? mageUIRenderer.spellNameColorOnCast
                    : mageUIRenderer.spellNameColorOnCooldown,
                showStatus = !spellToCast.canBeCast || spellToCast.isInCast,
                statusColor = spellToCast.isInCast
                    ? mageUIRenderer.spellBackgroundColorInCast
                    : mageUIRenderer.spellBackgroundColorOnCooldown,
                statusFillAmount = spellToCast.recastDelay > 0f
                    ? Mathf.Clamp01(1f - spellToCast.cooldown / spellToCast.recastDelay)
                    : 1f,
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

                if (!state.isVisible)
                {
                    if (ui.spellPosition.gameObject.activeSelf)
                        ui.spellPosition.gameObject.SetActive(false);
                    continue;
                }

                // Position
                ui.spellPosition.gameObject.SetActive(true);
                ui.spellPosition.anchoredPosition = state.position;

                // Spell name
                ui.spellName.color = state.nameColor;

                // Spell status
                if (state.showStatus)
                {
                    ui.spellStatus.gameObject.SetActive(true);
                    ui.spellStatus.color = state.statusColor;
                    ui.spellStatus.fillAmount = state.statusFillAmount;
                }
                else
                {
                    ui.spellStatus.gameObject.SetActive(false);
                }

                // Spell Inputs
                for (int i = 0; i < ui.spellInputs.Count; i++)
                {
                    if (i < state.directions.Length)
                    {
                        ui.spellInputs[i].gameObject.SetActive(true);
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

                        ui.spellInputs[i].color = color;
                        ui.spellInputs[i].rectTransform.rotation = Quaternion.Euler(0, 0, angle);
                    }
                    else
                    {
                        ui.spellInputs[i].gameObject.SetActive(false);
                    }
                }
            }
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
                _scrollSpell = _spellsUI.anchoredPosition - new Vector2(0f, transitionDistance);
                _spellsUI.anchoredPosition = _scrollSpell;
                return;
            }

            // Close Grimory UI
            if (grimoireUI.sizeDelta.y > GRIMOIRE_UI_MIN_HEIGHT)
            {
                _scrollGrimoryUISize.y -= transitionDistance;
                grimoireUI.sizeDelta = _scrollGrimoryUISize;
                _spellsDisplayUI.sizeDelta = _scrollGrimoryUISize - new Vector2(0, 20);
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
    }
}