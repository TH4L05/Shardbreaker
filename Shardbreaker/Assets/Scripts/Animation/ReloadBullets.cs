using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadBullets : StateMachineBehaviour
{
    BulletArmRevolver bulletArmRevolver;
    [Range(0,3)]
    public float delay = 0;
    float time = 0;
    bool isReloaded = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        time = 0;
        isReloaded = false;
    }

    void Reload()
    {
        if(bulletArmRevolver == null)
        {
            bulletArmRevolver = GameObject.Find("Revolver").GetComponent<BulletArmRevolver>();
        }

        bulletArmRevolver.UpdateAmmoPlus();
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(time >= delay && !isReloaded)
        {
            Reload();
            isReloaded = true;
        }

        time += Time.deltaTime;
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
