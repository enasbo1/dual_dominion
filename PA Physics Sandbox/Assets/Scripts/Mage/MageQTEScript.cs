using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Mage
{
    public class MageQTEScript : MonoBehaviour
    {
        public enum ControllerInputType
        {
            DualShock,
            Buttons,
            Both,
        }
        
        // public Transform playerTransform;
        [Header("GameObjects needed")]
        public PlayerInput player;
        public Slider timeBarSlider;
        public SpellManager spellManager;
        public RectTransform inputsUI;
        
        [Header("QTE values")]
        public ControllerInputType controllerInputType;
        [Range(0f, 0.9f)]
        public float crossDetectionSensibility = 0.7f;
        public float timeLimit = 15f;
        public float bonusTimePerInput = 0.1f;


        private readonly List<Image> _playerInputs = new List<Image>();
        private Vector2 _inputsStartPosition;
        private SpellDirections _inputCurrent = SpellDirections.None;
        private SpellDirections _inputPrevious = SpellDirections.None;
        private int _inputStep;
        private float _inputTimer;
        private Vector2 _moveVector;
        private List<Spell> _spellsAvailable = new List<Spell>();
        private Spell? _spellParent;
        private bool _isIncanting;
        private float _timeBarWidth;
        private float _timeBarWidthMax;
        
        private InputAction _incantationTrigger;
        private InputAction _actionMove;
        private InputAction _incantationMove;

        private void Start()
        {
            _inputsStartPosition = inputsUI.anchoredPosition;
            foreach (Transform child in inputsUI.transform)
            {
                _playerInputs.Add(child.GetComponent<Image>());
            }
            
            if (_playerInputs.Count < 15)
            {
                throw new Exception("Number of inputs insufficient");
            }

            _incantationTrigger = player.actions["incantationTrigger"];
            _incantationTrigger.started += _ => IncantationRestart();
            _incantationTrigger.canceled += _ => CastSpell(_spellParent);

            _actionMove = player.actions["move"];
            _incantationMove = player.actions["IncantationMove"];
            
            switch (controllerInputType)
            {
                case ControllerInputType.DualShock:
                    _actionMove.canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    _actionMove.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    break;
                case ControllerInputType.Buttons:
                    _incantationMove.canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    _incantationMove.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    break;
                case ControllerInputType.Both:
                    _incantationMove.canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    _actionMove.canceled += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    _incantationMove.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    _actionMove.performed += ctx => _moveVector = ctx.ReadValue<Vector2>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
     
            _spellsAvailable = spellManager.GetSpells().GetRange(0, spellManager.GetSpells().Count);
            IncantationEnd();
        }
        
        private void IncantationRestart()
        {
            _inputPrevious = SpellDirections.None;
            _isIncanting = true;
        }

        private void IncantationEnd()
        {
            _isIncanting = false;
            
            _inputStep = 0;
            _inputTimer = 0f;
            
            _spellsAvailable.Clear();
            _spellsAvailable.AddRange(spellManager.GetSpells());
            
            _playerInputs.ForEach(input => input.gameObject.SetActive(false));
            inputsUI.anchoredPosition = _inputsStartPosition;
        }

        private void IncantationCheck()
        {
            if (!_isIncanting) return;

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

        private void InputDisplay(string hexColor, float rotationAngle)
        {
            if (!ColorUtility.TryParseHtmlString(hexColor, out Color color)) return;
            
            _playerInputs[_inputStep].gameObject.SetActive(true);
            _playerInputs[_inputStep].color = color;
            _playerInputs[_inputStep].rectTransform.rotation = Quaternion.Euler(0, 0, rotationAngle);
        }

        private void Update()
        {
            IncantationCheck();
        }
        
        private void IncantationDisplay()
        {
            timeBarSlider.value = (timeLimit -_inputTimer) / timeLimit;
            timeBarSlider.gameObject.SetActive(_inputTimer > 0f);

            if (!_isIncanting && _inputStep == 0) return;
            
            if (_inputPrevious == _inputCurrent) return;
            
            switch (_inputCurrent)
            {
                case SpellDirections.Up:
                    InputDisplay("#FFB600", 0f);
                    break;
                case SpellDirections.Down:
                    InputDisplay("#009DFF", 180f);
                    break;
                case SpellDirections.Left:
                    InputDisplay("#00FF15", 90f);
                    break;
                case SpellDirections.Right:
                    InputDisplay("#FF0080", -90f);
                    break;
                case SpellDirections.None:
                default:
                    break;
            }
        }
        
        private void LateUpdate()
        {
            IncantationDisplay();
        }
        
        private void Incanting()
        {
            // Prevent triggering the input while in it
            if (_inputPrevious == _inputCurrent) return;
            
            _inputPrevious = _inputCurrent;

            if (_inputCurrent == SpellDirections.None) return;
            
            _spellsAvailable = _spellsAvailable.Where(spell =>
            {
                if (spell.inputs.Count <= _inputStep) return false;

                // If input not for this spell, remove it from the available ones
                if (_inputCurrent != spell.inputs[_inputStep]) return false;

                if (_inputStep == spell.inputs.Count - 1) _spellParent = spell;
                
                return true;
            }).ToList();

            if (_isIncanting)
            {
                _inputStep += 1;
                _inputTimer -= bonusTimePerInput;
                _inputTimer = Mathf.Max(_inputTimer, 0);

                if (_inputStep > 6)
                {
                    Vector2 inputMovements = inputsUI.anchoredPosition + new Vector2(-40f, 0f);
                    inputsUI.anchoredPosition = inputMovements;
                }
            }
        }

        private void CastSpell(Spell? spellToCast, bool castAsError = false)
        {
            _isIncanting = false;
            
            if (spellToCast == null) return;


            if (castAsError) spellToCast.Value.CastFailure();
            else spellToCast.Value.Cast(); 
            
            _spellParent = null;
            IncantationEnd();
        }
                
        private void FixedUpdate()
        {
            if (_inputStep > 0)
            {
                _inputTimer += Time.deltaTime;
                
            }
            if (_isIncanting)
            {
                Incanting();
            }
            
            // Condition to fail an incantation
            if (!(_spellsAvailable.Count > 0) || _inputTimer >= timeLimit)
            {
                CastSpell(_spellParent, true);
                IncantationEnd();
                Debug.Log("Failed");
            }
        }
    }
}
