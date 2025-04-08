using System;
using Actions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Move
{
    public class MageController : MonoBehaviour
    {
        [SerializeField] private Animator characterAnimator;
        [SerializeField] private Rigidbody characterBody;
        [FormerlySerializedAs("player")] public PlayerInput playerInputs;
        [SerializeField] private ActionManager actionManager;
        [SerializeField] private LayerMask opponentLayerMask;
        [SerializeField] private Transform characterTransform;
        [SerializeField] private MoveScript moveScript;

        private int _animationState;
        private static readonly int WalkState = Animator.StringToHash("WalkState");
        private static readonly int MidAir = Animator.StringToHash("mid-air");
        private static ShieldAction _shieldAction;
        private Vector2 _walkDirection = Vector2.zero;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _shieldAction = new ShieldAction(characterAnimator);
            characterAnimator.SetInteger(WalkState, 0);
            playerInputs.actions["move"].performed += ctx => SetWalkState(ctx.ReadValue<Vector2>());
            playerInputs.actions["move"].canceled += _ => StopWalking();
            playerInputs.actions["attack"].started += _ => Attack();
            playerInputs.actions["jump"].started += _ => Jump();
            playerInputs.actions["shield"].started += _ => _shieldAction.launch();
            playerInputs.actions["shield"].canceled += _ => _shieldAction.end();
        }

        private void StopWalking()
        {
            characterAnimator.SetInteger(WalkState, 0);
            _walkDirection = Vector2.zero;
            _animationState = 0;
        }
        
        private void AirControl(Vector3 directionIntent, float speedFactor)
        {
           
            if (directionIntent.magnitude != 0f){
                var normalizedDirection = directionIntent.normalized;

                var nV = characterBody.linearVelocity.normalized;
                var reflux = -Vector3.Dot(normalizedDirection, nV);
                
                reflux = (reflux < 2f) ? reflux : 2f;
                if (characterBody.linearVelocity.sqrMagnitude <
                    (moveScript.movementSpeed * moveScript.movementSpeed * speedFactor * speedFactor))
                    reflux = (reflux > 0f) ? reflux : 0f;
                else
                    if (reflux < 0f)
                        reflux *= 1.5f;
                characterBody.AddForce((normalizedDirection + nV * reflux) * (moveScript.movementSpeed * speedFactor * 10), ForceMode.Acceleration);
            }
        }
        
        private void SetWalkState(Vector2 walkInput)
        {
            _walkDirection = walkInput;
            if (walkInput == Vector2.zero)
            {
                characterAnimator.SetInteger(WalkState, 0);
            }
            else
            {
                if ((Math.Abs(walkInput.y)+.3) >= Math.Abs(walkInput.x))
                    SetWalkStateTo(walkInput.y > 0 ? 1 : 3);
                else
                    SetWalkStateTo(walkInput.x > 0 ? 2 : 4);
            }
        }

        private void SetWalkStateTo(int walkState)
        {
            characterAnimator.SetInteger(WalkState, walkState);
            _animationState = walkState;
        }

        private void Attack()
        {
            actionManager.AddAction(new MeleeAttackAction(characterAnimator, characterTransform, opponentLayerMask));
        }

        private void Jump()
        {
            if (!characterAnimator.GetBool(MidAir))
                actionManager.AddAction(new JumpAction(characterBody, 15));
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


        public void FixedUpdate()
        {
            if (characterAnimator.GetBool(MidAir))
                AirControl(characterTransform.rotation * new Vector3(_walkDirection.x, 0, _walkDirection.y), 0.5f);

        }
    }

}
