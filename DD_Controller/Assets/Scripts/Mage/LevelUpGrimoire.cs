using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mage
{
    public class SpellUI
    {
        public Image spellIcon;
        public List<Image> spellInputs;
        public TextMeshProUGUI spellName;
        public RectTransform spellPosition;
        public Image spellStatus;
    }


    public class LevelUpGrimoire : MonoBehaviour
    {
        private const float SPELL_UI_HEIGHT = 70f;
        public MageUIRendererScript mageUIRenderer;
        public SpellManager spellManager;
        public RectTransform levelUpGrimoireUI;

        [Range(0.1f, 5f)] public float transitionDistance = 1f;

        private readonly Dictionary<int, SpellUI> _spellsForSpellsUI = new Dictionary<int, SpellUI>();

        private float _grimoryOpeningTime;

        private Vector2 _scrollSpell;
        private List<Spell> _spellsToUnlock = new List<Spell>();

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
            _spellsDisplayUI = levelUpGrimoireUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUI = _spellsDisplayUI.GetChild(0).GetComponentInChildren<RectTransform>();
            _spellsUIStartPosition = _spellsUI.anchoredPosition;

            SetAllSpellsInUI(spellManager.GetSpells());
            _spellsToUnlock = spellManager.spellsToUnlock;
        }

        private void Update()
        {
            RefreshSpellsUI();
            _spellsUIEndPosition = _spellsUIStartPosition + new Vector2(0, (_spellsToUnlock.Count - 2) * 70);

            if (spellManager.isIncanting) return;
            float transitionSpeed = Time.fixedDeltaTime / transitionDistance;
            _timeLimit = SPELL_UI_HEIGHT * ((_spellsToUnlock.Count + 1 / transitionDistance) * transitionSpeed);
        }


        private void FixedUpdate()
        {
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

            if (spellManager.isIncanting) return;

            _timer += Time.deltaTime;

            // Scroll spells
            if (_spellsUI.anchoredPosition.y >= _spellsUIEndPosition.y) return;
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

        private static void RefreshSpellUIStatus(Image spellStatus)
        {
            spellStatus.gameObject.SetActive(false);
        }

        private void RefreshSpellNameUI(SpellUI spellUI)
        {
            spellUI.spellName.color = mageUIRenderer.spellNameColorOnCast;
        }

        private void RefreshSpellsUI()
        {
            Spell spellToUnlock = spellManager.spellToCast;
            SpellUI spellToUnlockUI = _spellsForSpellsUI[spellToUnlock.id];
            int spellToUnlockId = spellToUnlock.id;

            int i = 1;
            foreach (KeyValuePair<int, SpellUI> spellForSpellUI in _spellsForSpellsUI)
            {
                int spellId = spellForSpellUI.Key;
                SpellUI spellUI = spellForSpellUI.Value;

                if (_spellsToUnlock.All(s => s.id != spellId))
                {
                    spellUI.spellPosition.gameObject.SetActive(false);
                    continue;
                }

                if (spellId == spellToUnlockId) continue;

                Spell spell = _spellsToUnlock.First(s => s.id == spellId);

                RefreshSpellUIPosition(spellUI.spellPosition, i);

                RefreshSpellNameUI(spellUI);
                RefreshSpellUIStatus(spellUI.spellStatus);
                RefreshInputs(spellUI.spellInputs, spell);

                i++;
            }

            RefreshSpellUIPosition(spellToUnlockUI.spellPosition, 0);

            RefreshSpellNameUI(spellToUnlockUI);
            RefreshSpellUIStatus(spellToUnlockUI.spellStatus);
            RefreshInputs(spellToUnlockUI.spellInputs, spellToUnlock);
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

            _timer = 0;
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