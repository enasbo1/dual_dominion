using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace script

{
    public class Animation_Changer : MonoBehaviour
    {
        private static readonly int WalkState = Animator.StringToHash("WalkState");
        public Animator characterAnimator;
        public PlayerInput player;

    
    
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            characterAnimator.SetInteger(WalkState, 0);
            player.actions["move"].performed += ctx => setWalkState(ctx.ReadValue<Vector2>());
            player.actions["move"].canceled += ctx => characterAnimator.SetInteger(WalkState, 0);;
        }

        private void setWalkState(Vector2 walkInput)
        {
            if (walkInput == Vector2.zero)
            {
                characterAnimator.SetInteger(WalkState, 0);
            }
            else
            {
                if (Math.Abs(walkInput.y) >= Math.Abs(walkInput.x))
                    characterAnimator.SetInteger(WalkState, walkInput.y > 0 ? 1 : 3);
                else
                    characterAnimator.SetInteger(WalkState, walkInput.x > 0 ? 2 : 4);
            }
        }
    }
}
