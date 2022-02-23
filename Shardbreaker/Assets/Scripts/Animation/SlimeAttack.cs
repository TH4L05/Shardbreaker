using UnityEngine;

public class SlimeAttack : StateMachineBehaviour
{
    public GameObject obj;
    public Shoot shoot;
    public float targetdistanceOnAttack;

     //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        /*var particle = obj.GetComponent<ParticleSystem>();

        if (particle.isPlaying)
        {
            particle.Stop();
        }
        particle.Play();*/

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        /*var particle = obj.GetComponent<ParticleSystem>();
        particle.Stop();
        shoot.CreateBulletInstanceEnemy(targetdistanceOnAttack + (targetdistanceOnAttack / 3));*/
    }
}
