using UnityEngine;

public class ReloadAnimation : StateMachineBehaviour
{
    public Animator reloadAnimator;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        reloadAnimator = GameObject.Find("VFX_Revolver_Reload").GetComponent<Animator>();
        reloadAnimator.SetTrigger("EnergyToHand");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //reloadAnimator.SetTrigger("OnReload");
    }
}
