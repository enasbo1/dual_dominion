using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mage.SpellListener
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

        private readonly Dictionary<int, SpellUI> _spellsForSpellsUI = new();

        private Spell _grimoireSpell;
        private float _grimoryOpeningTime;
        private Vector2 _scrollGrimoryUISize;

        private Vector2 _scrollSpell;
        private List<Spell> _spellsAvailable = new();

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
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsAvailable.Count - 2) * 70);

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
            if (_spellsUI.anchoredPosition.y > _spellsUIEndPosition.y + SPELL_UI_HEIGHT)
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

        private static void RefreshSpellUIPosition(RectTransform spellUI, int spellIndex)
        {
            spellUI.gameObject.SetActive(true);
            Vector2 spellUINewPosition = spellUI.anchoredPosition;
            spellUINewPosition.y = -2.5f - spellIndex * SPELL_UI_HEIGHT;
            spellUI.anchoredPosition = spellUINewPosition;
        }

        private static void RefreshSpellUIStatus(Image spellStatus, Spell spell, MageUIRendererScript mageUIRenderer)
        {
            if (spell.canBeCast)
            {
                if (spell.isInCast)
                {
                    spellStatus.gameObject.SetActive(true);
                    spellStatus.color = mageUIRenderer.spellBackgroundColorInCast;
                    return;
                }

                spellStatus.gameObject.SetActive(false);
                return;
            }

            spellStatus.gameObject.SetActive(true);
            spellStatus.color = mageUIRenderer.spellBackgroundColorOnCooldown;
            spellStatus.fillAmount =
                spell.recastDelay > 0f ? Mathf.Clamp01(1f - spell.cooldown / spell.recastDelay) : 1f;
        }

        private void RefreshSpellNameUI(SpellUI spellUI, Spell spell)
        {
            spellUI.spellName.color = spell.canBeCast
                ? mageUIRenderer.spellNameColorOnCast
                : mageUIRenderer.spellNameColorOnCooldown;
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

                if (_spellsAvailable.All(s => s.id != spellId))
                {
                    spellUI.spellPosition.gameObject.SetActive(false);
                    continue;
                }

                if (spellId == spellToCastId) continue;

                Spell spell = _spellsAvailable.First(s => s.id == spellId);
                if (spell.isHidden)
                {
                    spellUI.spellPosition.gameObject.SetActive(false);
                    continue;
                };

                RefreshSpellUIPosition(spellUI.spellPosition, i);

                RefreshSpellNameUI(spellUI, spell);
                RefreshSpellUIStatus(spellUI.spellStatus, spell, mageUIRenderer);
                RefreshInputs(spellUI.spellInputs, spell);

                i++;
            }

            RefreshSpellUIPosition(spellToCastUI.spellPosition, 0);

            RefreshSpellNameUI(spellToCastUI, spellToCast);
            RefreshSpellUIStatus(spellToCastUI.spellStatus, spellToCast, mageUIRenderer);
            RefreshInputs(spellToCastUI.spellInputs, spellToCast);
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