using UnityEngine;

public class FlowerPetalAnimTestScript : MonoBehaviour
{
    private static readonly int PetalState = Animator.StringToHash("IsPetalDead");
    private static readonly int PetalDamaged = Animator.StringToHash("PetalDamaged");
    [SerializeField] private Animator petalAnimator;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        petalAnimator.SetBool(PetalState, false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            petalAnimator.SetBool(PetalState, !petalAnimator.GetBool(PetalState));
        }
        
        if (Input.GetKeyDown(KeyCode.V))
        {
            petalAnimator.SetTrigger(PetalDamaged);
        }
    }
}
