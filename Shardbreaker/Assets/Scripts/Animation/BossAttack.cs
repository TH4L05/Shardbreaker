using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class BossAttack : StateMachineBehaviour
{
    public bool isBoss;
    private EnemySlimeBoss slimeBoss;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //animator.SetBool("onAttack2", true);

        if (isBoss)
        {
            animator.SetBool("finishAttack", false);
            
        }


        if (!isBoss)
        {
            var enemy = animator.gameObject.GetComponentInParent<EnemySlime>();
            enemy.onAlternativeAttack2 = false;

            enemy.onAttack2 = false;
            animator.SetBool("finishAttack", true);
            animator.SetBool("onAttack2", false);
        }      
    }


}
