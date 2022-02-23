using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdle : StateMachineBehaviour
{
    public int minValue = 0;
    public int maxValue = 1;
    Enemy enemy;
    int randomValue;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        enemy = animator.gameObject.GetComponentInParent<Enemy>();
        randomValue = Random.Range(minValue, maxValue);
        animator.SetInteger("idleRandom", randomValue);
        //Debug.Log(random);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (randomValue > 7)
        {
            enemy.state = Enemy.State.MoveToPosition;
        }
    }
}
