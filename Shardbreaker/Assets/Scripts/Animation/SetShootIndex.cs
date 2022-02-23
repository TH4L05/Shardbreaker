using UnityEngine;

public class SetShootIndex : StateMachineBehaviour
{
    [SerializeField] private int shootIndex = 0;
    [SerializeField] private int shootIndexMax = 3;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        shootIndex++;

        if (shootIndex > shootIndexMax)
        {
            shootIndex = 1;
        }
        animator.SetInteger("shootIndex", shootIndex);     
    }
}
