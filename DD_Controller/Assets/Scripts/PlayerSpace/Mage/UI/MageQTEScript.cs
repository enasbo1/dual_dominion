using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PlayerSpace.Mage.UI
{
    public class MageQTEScript : WithEndMonoBehavior
    {
        public enum ControllerInputType
        {
            DualShock,
            Buttons,
            Both
        }

        [Header("GameObjects needed")] public PlayerInput playerInputs;

        public MageDealer mageDealer;
        public SpellManager spellManager;
        public MageUIRendererScript mageUIRenderer;
        public Slider timeBarSlider;
        public RectTransform inputsUI;

        [Header("QTE values")] public ControllerInputType controllerInputType;
        [Range(0f, 0.9f)] public float crossDetectionSensibility = 0.7f;
        public float timeLimit = 15f;
        public float bonusTimePerInput = 0.1f;


        private readonly List<Image> _inputsPerformedUI = new List<Image>();
        private InputAction _actionMove;
        private InputAction _incantationMove;

        private InputAction _incantationTrigger;
        private SpellDirections _inputCurrent = SpellDirections.None;
        private SpellDirections _inputPrevious = SpellDirections.None;
        private Vector2 _inputsStartPosition;
        private int _inputStep;
        private float _inputTimer;
        private Vector2 _moveVector;

        private List<Spell> _spellsAvailable = new List<Spell>();
        private List<Spell> _spellsToUnlock = new List<Spell>();
        private InputAction _spellTrigger;

        private float _timeBarWidth;
        private float _timeBarWidthMax;

        private void Start()
        {
            _inputsStartPosition = inputsUI.anchoredPosition;

            foreach (Transform inputUI in inputsUI.transform) _inputsPerformedUI.Add(inputUI.GetComponent<Image>());

            int longestInputs = spellManager.GetSpells().Max(spell => spell.inputs.Count);
            if (_inputsPerformedUI.Count < longestInputs) throw new Exception("Number of inputs insufficient");

            _incantationTrigger = playerInputs.actions["IncantationTrigger"];
            _incantationTrigger.started += ToBeCleanedAction(
                _ => IncantationRecover(),
                a => _incantationTrigger.started -= a
            );

            _spellTrigger = playerInputs.actions["CastSpell"];
            _spellTrigger.started += ToBeCleanedAction(
                _ => { if (spellManager.isIncanting) CastSpell(); },
                a => _spellTrigger.started -= a
            );

            _actionMove = playerInputs.actions["Move"];
            _incantationMove = playerInputs.actions["IncantationMove"];

            switch (controllerInputType)
            {
                case ControllerInputType.DualShock:
                    _actionMove.canceled += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _actionMove.canceled -= a
                    );
                    _actionMove.performed += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _actionMove.performed -= a
                    );
                    break;
                case ControllerInputType.Buttons:
                    _incantationMove.canceled += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _incantationMove.canceled -= a
                    );
                    _incantationMove.performed += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _incantationMove.performed -= a
                    );
                    break;
                case ControllerInputType.Both:
                    _incantationMove.canceled += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _incantationMove.canceled -= a
                    );
                    _actionMove.canceled += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _incantationMove.canceled -= a
                    );
                    _incantationMove.performed += ToBeCleanedAction(ctx => _moveVector = ctx.ReadValue<Vector2>(),
                        a => _incantationMove.performed -= a
                    );
                    _actionMove.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _spellsAvailable = spellManager.spellsAvailable;
            _spellsToUnlock = spellManager.spellsToUnlock;
            IncantationEnd();
        }

        private void Update()
        {
            IncantationCheck();
            IncantationDisplay();
            if (spellManager.isIncanting) Incanting();
        }

        private void FixedUpdate()
        {
            if (_inputStep > 0) _inputTimer += Time.deltaTime;
            
            // Condition to fail an incantation
            if (_spellsAvailable.Count > 0 && _spellsToUnlock.Count > 0 && _inputTimer < timeLimit) return;

            CastSpell(true);
        }

        private void IncantationEnd()
        {
            spellManager.isIncanting = false;

            _inputStep = 0;
            _inputTimer = 0f;

            spellManager.ResetSpellsAvailable();
            spellManager.ResetSpellsToUnlock();
            spellManager.spellToCast = spellManager.defaultSpell;

            _inputsPerformedUI.ForEach(input => input.gameObject.SetActive(false));
            inputsUI.anchoredPosition = _inputsStartPosition;
        }

        private void CastSpell(bool castAsError = false)
        {
            Spell spellToCast = spellManager.spellToCast;
            
            if (spellToCast == null)
            {
                IncantationEnd();
                return;
            }
            
            if (mageDealer.skillsToUnlock >= 1)
            {
                if (!castAsError && spellToCast.id != spellManager.defaultSpell.id)
                {
                    spellToCast.UnlockSpell();
                    mageDealer.skillsToUnlock -= 1;
                }

                IncantationEnd();
                return;
            }
            
            if (castAsError) spellToCast.CastFailure();
            else if (spellToCast.canBeCast) spellToCast.Cast();

            IncantationEnd();
        }

        public bool GetIncantingState()
        {
            return spellManager.isIncanting;
        }

        private void IncantationRecover()
        {
            _inputPrevious = SpellDirections.None;
            spellManager.isIncanting = true;
        }
        
        private void IncantationCheck()
        {
            if (!spellManager.isIncanting) return;

            if (_moveVector.y > crossDetectionSensibility)
                _inputCurrent = SpellDirections.Up;
            else if (_moveVector.y < -crossDetectionSensibility)
                _inputCurrent = SpellDirections.Down;
            else if (_moveVector.x > crossDetectionSensibility)
                _inputCurrent = SpellDirections.Right;
            else if (_moveVector.x < -crossDetectionSensibility)
                _inputCurrent = SpellDirections.Left;
            else
                _inputCurrent = SpellDirections.None;
        }

        private void InputDisplay(Color color, float rotationAngle)
        {
            _inputsPerformedUI[_inputStep].gameObject.SetActive(true);
            _inputsPerformedUI[_inputStep].color = color;
            _inputsPerformedUI[_inputStep].rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }

        private void IncantationDisplay()
        {
            timeBarSlider.value = (timeLimit - _inputTimer) / timeLimit;
            timeBarSlider.gameObject.SetActive(_inputTimer > 0f);

            if (!spellManager.isIncanting && _inputStep == 0) return;

            if (_inputPrevious == _inputCurrent) return;

            switch (_inputCurrent)
            {
                case SpellDirections.Up:
                    InputDisplay(mageUIRenderer.upArrowColor, 0f);
                    break;
                case SpellDirections.Right:
                    InputDisplay(mageUIRenderer.rightArrowColor, -90f);
                    break;
                case SpellDirections.Down:
                    InputDisplay(mageUIRenderer.downArrowColor, 180f);
                    break;
                case SpellDirections.Left:
                    InputDisplay(mageUIRenderer.leftArrowColor, 90f);
                    break;
                case SpellDirections.None:
                default:
                    break;
            }
        }

        private void Incanting()
        {
            // Prevent triggering the input while in it
            if (_inputPrevious == _inputCurrent) return;

            _inputPrevious = _inputCurrent;
            if (_inputCurrent == SpellDirections.None) return;

            if (mageDealer.skillsToUnlock > 0)
            {
                spellManager.SetSpellsToUnlock(_spellsToUnlock.Where(spell =>
                {
                    if (spell.inputs.Count <= _inputStep) return false;

                    // If input not for this spell, remove it from the unlockable ones
                    if (_inputCurrent != spell.inputs[_inputStep]) return false;

                    if (_inputStep == spell.inputs.Count - 1) spellManager.spellToCast = spell;

                    return true;
                }).ToList());
            }
            else
            {
                spellManager.SetSpellsAvailable(_spellsAvailable.Where(spell =>
                {
                    if (spell.inputs.Count <= _inputStep) return false;

                    // If input not for this spell, remove it from the available ones
                    if (_inputCurrent != spell.inputs[_inputStep]) return false;

                    if (_inputStep == spell.inputs.Count - 1) spellManager.spellToCast = spell;

                    return true;
                }).ToList());
            }

            if (!spellManager.isIncanting) return;

            _inputStep += 1;
            _inputTimer -= bonusTimePerInput;
            _inputTimer = Mathf.Max(_inputTimer, 0);

            if (_inputStep <= 6) return;

            Vector2 inputMovements = inputsUI.anchoredPosition + new Vector2(-40f, 0f);
            inputsUI.anchoredPosition = inputMovements;
        }
    }
}