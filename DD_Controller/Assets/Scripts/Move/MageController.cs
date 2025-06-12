using System;
using Actions;
using Monster;
using Shared;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Move
{
    public enum MoveMode
    {
        ThirdPerson,
        UpView
    }

    public class MageController : MonoBehaviour
    {
        private static readonly int WalkState = Animator.StringToHash("WalkState");
        private static readonly int MidAir = Animator.StringToHash("mid-air");
        private static ShieldAction _shieldAction;
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private Rigidbody characterBody;
        [FormerlySerializedAs("player")] public PlayerInput playerInputs;
        [SerializeField] private ActionManager actionManager;
        [SerializeField] private LayerMask opponentLayerMask;
        [SerializeField] private Transform directionMain;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private MoveScript moveScript;
        [SerializeField] public MoveMode moveMode = MoveMode.ThirdPerson;
        public MonsterLifeManager lifeScript;
        public SensorScript sensorScript;

        private int _animationState;
        private Vector2 _inputDirection = Vector2.zero;
        private Vector2 _walkDirection = Vector2.zero;
        private JumpAction _jumpAction;

        private MeleeAttackAction _attackAction;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _jumpAction = new JumpAction(characterBody, 15, moveScript);
            _shieldAction = new ShieldAction(characterAnimator, characterTransform, sensorScript);
            characterAnimator.SetInteger(WalkState, 0);
            playerInputs.actions["move"].performed += ctx => _inputDirection = ctx.ReadValue<Vector2>();
            playerInputs.actions["move"].canceled += _ => StopWalking();
            playerInputs.actions["attack"].started += _ => Attack();
            playerInputs.actions["jump"].started += _ => Jump();
            playerInputs.actions["shield"].started += _ => _shieldAction.launch();
            playerInputs.actions["shield"].canceled += _ => _shieldAction.end();

            _attackAction = new MeleeAttackAction(characterAnimator, characterTransform, sensorScript, lifeScript);
        }


        public void FixedUpdate()
        {
            SetWalkState();
            if (characterAnimator.GetBool(MidAir))
                AirControl(characterTransform.rotation * new Vector3(_walkDirection.x, 0, _walkDirection.y), 0.5f);
        }

        private void StopWalking()
        {
            characterAnimator.SetInteger(WalkState, 0);
            _walkDirection = Vector2.zero;
            _inputDirection = Vector2.zero;
            _animationState = 0;
        }

        private void AirControl(Vector3 directionIntent, float speedFactor)
        {
            if (directionIntent.magnitude == 0f) return;
            Vector3 normalizedDirection = directionIntent.normalized;

            Vector3 nV = characterBody.linearVelocity.normalized;
            float reflux = -Vector3.Dot(normalizedDirection, nV);

            reflux = reflux < 2f ? reflux : 2f;
            if (characterBody.linearVelocity.sqrMagnitude <
                moveScript.movementSpeed * moveScript.movementSpeed * speedFactor * speedFactor)
                reflux = reflux > 0f ? reflux : 0f;
            else if (reflux < 0f)
                reflux *= 1.5f;
            characterBody.AddForce((normalizedDirection + nV * reflux) * (moveScript.movementSpeed * speedFactor * 10),
                ForceMode.Acceleration);
        }

        private void SetWalkState()
        {
            _walkDirection = moveMode switch
            {
                MoveMode.ThirdPerson => _inputDirection,
                MoveMode.UpView => Vector2Extension.RotateDeg(_inputDirection, directionMain.rotation.eulerAngles.y),
                _ => _inputDirection
            };
            if (_inputDirection == Vector2.zero)
            {
                characterAnimator.SetInteger(WalkState, 0);
            }
            else
            {
                if (Math.Abs(_walkDirection.y) + .3 >= Math.Abs(_walkDirection.x))
                    SetWalkStateTo(_walkDirection.y > 0 ? 1 : 3);
                else
                    SetWalkStateTo(_walkDirection.x > 0 ? 2 : 4);
            }
        }

        private void SetWalkStateTo(int walkState)
        {
            characterAnimator.SetInteger(WalkState, walkState);
            _animationState = walkState;
        }

        private void Attack()
        {
            actionManager.AddAction(_attackAction);
        }

        private void Jump()
        {
            if (!characterAnimator.GetBool(MidAir))
                actionManager.AddAction(_jumpAction);
        }


        public float GetTargetWalkDirection()
        {
            return Mathf.Atan2(_walkDirection.x, _walkDirection.y) * Mathf.Rad2Deg;
        }

        public float? GetCurrentWalkDirection()
        {
            if (_animationState == 0) return null;
            return (_animationState - 1) * 90;
        }
    }
}