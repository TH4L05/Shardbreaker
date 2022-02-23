using UnityEngine;
using UnityEngine.AI;

public class EnemyBlink : Enemy
{
    [SerializeField] private float blinkStartDistance = 4f;
    [SerializeField] private float blinkEndDistance = 2f;
    [SerializeField] private float attackDistance = 10f;
    [SerializeField] private float attackspeedmultiply = 2f;
    [SerializeField] private MeshRenderer meshRenderer;
    bool hasAttackPos;
    bool readyForAttack;
    bool attack;
    //bool onAttack;
    Transform playerPosOnAttackStart;

    public override void StartSetup()
    {
        target = Game.instance.player.transform;
        state = State.MoveToPosition;
    }

    public override void UpdateEnemy()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (!attack)
        {
            meshRenderer.enabled = true;
            if (distance < lookRange)
            {

                if (distance <= attackDistance)
                {
                    attack = true;
                    if (!hasAttackPos)
                    {
                        navAgent.SetDestination(transform.position);
                        state = State.MoveToAttackPosition;
                        
                    }
                }
                else
                {
                   state = State.Attack;
                }
            }
            else
            {
                state = State.MoveToPosition;
            }

        }

        CheckState();
    }

    public override void CheckState()
    {
        if (!health.IsDead)
        {
            float speedValue = navAgent.velocity.magnitude / navAgent.speed;

            switch (state)
            {
                case State.Idle:
                    break;
                case State.MoveToTarget:
                    LookAtTarget(target.position);
                    MoveToTarget(speedValue);
                    break;
                case State.MoveToPosition:
                    MoveToPosition(speedValue);
                    //LookAtTarget(randomTargetTransform);
                    break;
                case State.MoveToAttackPosition:
                    Debug.Log("<Color=lime>go to  Attack Position</color>");    
                    MoveToAttackPosition(speedValue);
                    break;
                case State.Attack:
                    playerPosOnAttackStart = target;
                    Attack();
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (!IsDead)
            {
                //animator.SetBool("isDead", true);
                navAgent.isStopped = true;
                IsDead = true;
                Death();
            }

        }
    }

    public void MoveToTarget(float speedValue)
    {
        navAgent.SetDestination(target.position);
    }

    private void MoveToPosition(float speedValue)
    {
        
        navAgent.speed = 4f;
        //animator.SetFloat("speed", speedValue);
        if (navAgent.remainingDistance <= 0.1f)
        {
            //navAgent.isStopped = false;
            if (idleTimeCurrent >= idleTime)
            {
                SetRandomTarget();
                idleTimeCurrent = 0f;
            }
            else
            {
                idleTimeCurrent += Time.deltaTime;
            }
        }
        if (navAgent.remainingDistance > 1f)
        {
            
        }
    }

    private void MoveToAttackPosition(float speedValue)
    {
        if (!readyForAttack)
        {
            hasAttackPos = true;
            readyForAttack = true;
            navAgent.speed = 15f;
            navAgent.SetDestination(AttackPosition());
            
        }
        if (navAgent.remainingDistance <= 0.1f)
        {
            state = State.Attack;
        }
    }

    public override void Attack()
    {
        float distance = Vector3.Distance(playerPosOnAttackStart.position, transform.position);

        if (!onAttack)
        {
            hasAttackPos = false;
            Debug.Log("<color=magenta><b>go for Target</b></color>");
            onAttack = true;
            //animator.SetTrigger("attack");
            LookAtTarget(playerPosOnAttackStart.position);
            navAgent.speed = 15f;
            navAgent.SetDestination(playerPosOnAttackStart.position);
        }
        else
        {
            if (distance < blinkStartDistance)
            {
                meshRenderer.enabled = false;
            }
            /*else if (distance < blinkEndDistance)
            {
                meshRenderer.enabled = true;
            }*/
            
            if (navAgent.remainingDistance <= blinkEndDistance)
            {
                Debug.Log("<color=teal><b>Ready for Attack</b></color>");
                meshRenderer.enabled = true;
                onAttack = false;
                readyForAttack = false;
                attack = false;
            }
        }

        
    }

    Vector3 AttackPosition()
    {
        Vector3 position = UnityEngine.Random.insideUnitSphere * attackDistance;
        position += transform.position;

        NavMesh.SamplePosition(position, out NavMeshHit hit, 20, 1);

        return hit.position;
    }


    public override void GetHit(float damage, DamageTypes.DamageType damageType)
    {
        base.GetHit(damage, damageType);
    }

    public override void Death()
    {
        base.Death();
    }

    private void OnDrawGizmos()
    {
        if (randomTargetTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(randomTargetTransform.position, 1);
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, attackDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), lookRange);
        }
    }
}
