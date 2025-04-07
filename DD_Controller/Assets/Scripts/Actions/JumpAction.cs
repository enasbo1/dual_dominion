using UnityEngine;

namespace actions
{
    public struct JumpAction : IDdAction
    {
        private Rigidbody rb;
        private Animator animator;
        private float jumpForce;
        public int id { get;  set;}
        
        private static readonly int Jump = Animator.StringToHash("Jump");

        JumpAction(Rigidbody rb, Animator animator, float jumpForce)
        {
            id = 0;
            this.rb = rb;
            this.animator = animator;
            this.jumpForce = jumpForce;
        }


        public void launch()
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger(Jump);
        }

        public bool update()
        {
            return true;
        }

        public void end()
        {
        }
    }
}