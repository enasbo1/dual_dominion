using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace script.move
{
    public class AnimationChanger : MonoBehaviour
    {
        public Animator characterAnimator;
        public PlayerInput player;

        private int _animationState;
        private static readonly int WalkState = Animator.StringToHash("WalkState");
        private static readonly int Melee = Animator.StringToHash("Melee");
        private Vector2 _walkDirection = Vector2.zero;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            characterAnimator.SetInteger(WalkState, 0);
            player.actions["move"].performed += ctx => SetWalkState(ctx.ReadValue<Vector2>());
            player.actions["move"].canceled += _ => StopWalking();
            player.actions["attack"].started += _ => Attack();
        }

        private void StopWalking()
        {
            characterAnimator.SetInteger(WalkState, 0);
            _walkDirection = Vector2.zero;
            _animationState = 0;
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
            characterAnimator.SetTrigger(Melee);
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
