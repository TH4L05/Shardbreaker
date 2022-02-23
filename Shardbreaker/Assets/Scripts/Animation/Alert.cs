using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alert : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("alert", false);    
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemy = animator.gameObject.GetComponentInParent<Enemy>();
        enemy.state = Enemy.State.MoveToTarget;
    }
}
