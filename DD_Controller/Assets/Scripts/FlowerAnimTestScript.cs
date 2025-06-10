using UnityEngine;

public class FlowerAnimTestScript : MonoBehaviour
{
    private static readonly int PetalState = Animator.StringToHash("PetalState");
    [SerializeField] private Animator characterAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterAnimator.SetInteger(PetalState, 0);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (characterAnimator.GetInteger(PetalState))
            {
                case 0:
                    characterAnimator.SetInteger(PetalState, 1);
                    break;
                case 1:
                    characterAnimator.SetInteger(PetalState, 2);
                    break;
                case 2:
                    characterAnimator.SetInteger(PetalState, 0);
                    break;
            }
        }
    }
}
