using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class SlimeAnimationEvents : MonoBehaviour
{

    public PlayableDirector attack2Playable;
    public NavMeshAgent navAgent;
    public Enemy enemyScriptRef;
    public Shoot shootScriptRef;
    public EnemySlimeBoss slimeBossRef;
    public bool isBoss;

    public void ProtectionPhase()
    {
        if(slimeBossRef.protectionPhase == false)
        {
            slimeBossRef.protectionPhase = true;

        }

    }

    public void PlayDirector()
    {
        attack2Playable.Play();
    }

    public void DisableNavAgent()
    {
        navAgent.enabled = false;
    }

    public void PlayAttack2Audio()
    {
        enemyScriptRef.attack2Audio.HandleEvent(enemyScriptRef.attack2Audio.gameObject);
    }

    public void EnableNavAgent()
    {
        navAgent.enabled = true;
    }

    public void ShootProjectile()
    {    
       shootScriptRef.CreateBulletInstanceEnemy(enemyScriptRef.targetdistanceOnAttack / enemyScriptRef.targetdistanceOnAttackOffset);
    }

    public void PlayWalkAudio()
    {
        enemyScriptRef.moveAudio.HandleEvent(enemyScriptRef.moveAudio.gameObject);
    }

    public void PlayDeathAudio()
    {
        enemyScriptRef.dieAudio.HandleEvent(enemyScriptRef.dieAudio.gameObject);
    }
}
