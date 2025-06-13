using UnityEngine;

public class FlowerStemAnimTestScript : MonoBehaviour
{
    private static readonly int StemDamaged = Animator.StringToHash("StemDamaged");
    [SerializeField] private Animator stemAnimator;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            stemAnimator.SetTrigger(StemDamaged);
        }
    }
}