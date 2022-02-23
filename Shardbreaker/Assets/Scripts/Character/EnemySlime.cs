using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySlime : Enemy
{
    bool raycastHit;
    
    public float attack2range = 2f;
    public bool hasAlternateAttack2Behaviour;
    public float timeBetweenAttack2Alternative = 3f;
    private float lastAttack2Alternative;
    public bool onAttack2;
    public bool onAlternativeAttack2;


    private void FixedUpdate()
    {
        raycastHit = RayCastTargetCheck();
    }

    public override void StartSetup()
    {
        state = State.Idle; 
    }

    public override void UpdateEnemy()
    {
        if (IsDead) return;
 
        float distance = DistanceCheck();
        //Debug.Log($"<color=red>ENEMY to PLAYER Distance = {distance}</color>");

        if (lastAttack2Alternative + timeBetweenAttack2Alternative <= Time.time)
        {
            //lastAttack2Alternative = Time.time;
            if (hasAlternateAttack2Behaviour && onChase)
            {
                onAlternativeAttack2 = true;
            }           
        }

        if (!onChase)
        {

            if (distance < lookRange)
            {

                if (distance <= minimumDectectRange)
                {
                    state = State.Alert;
                }
                else if (raycastHit && distance <= attackRange)
                {
                    state = State.Alert;
                }
            }
        }
        else
        {
            if (hasAlternateAttack2Behaviour) 
            {
                if (onAlternativeAttack2)
                {                   
                    animator.SetBool("onAttack2", true);
                    animator.SetBool("OnAttack", false);

                    if (distance <= 2f)
                    {
                        state = State.Attack;
                    }
                    else
                    {
                        state = State.MoveToTarget;
                    }
                }               
            }
            else
            {
                if (distance <= attackRange)
                {
                    state = State.Attack;
                }
                else
                {
                    animator.SetBool("OnAttack", false);
                    onAttack = false;
                    state = State.MoveToTarget;
                }
            }
        }

        if (!manualState)
        {
            CheckState();
        }
    }


    private float DistanceCheck()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        return distance;
    }

    public override void CheckState()
    {
        if (!health.IsDead)
        {
            switch (state)
            {
                case State.Idle:
                    moveToPosition = false;
                    break;
                case State.Alert:
                    Alert();
                    break;
                case State.MoveToTarget:
                    LookAtTargetSmooth(target.position, 1.5f);
                    //LookAtTarget(target.position);
                    MoveToTarget();
                    break;
                case State.MoveToPosition:
                    MoveToPosition();
                    break;
                case State.Attack:
                    if (onAlternativeAttack2)
                    {
                        Attack2Alternative();
                    }
                    else
                    {
                        LookAtTargetSmooth(target.position, 2f);
                        //LookAtTarget(target.position);
                        Attack();
                    }
                    break;
                case State.Death:
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (!IsDead)
            {               
                Death();
            }
        }
    }

    private void Alert()
    {
        if (!onChase)
        {
            //LookAtTargetSmooth(target.position, 10f);
            AlertEnemiesInRange();
            animator.SetBool("alert", true);
            onChase = true;
            animator.SetBool("onChase", true);
            AlertEnemiesInRange();
            EnemyStartAttack();
        }     
    }

    private void AlertEnemiesInRange()
    {
        if (!onChase)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange, EnemyMask);

            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].GetComponent<Enemy>().state = State.Alert;
            }
        }
    }

    private void MoveToPosition()
    {
        navAgent.isStopped = false;
        navAgent.speed = move_speed;
        navAgent.acceleration = move_speed * 2;
        navAgent.stoppingDistance = 1f;

        if (navAgent.remainingDistance < 0.1f)
        {
            if (!moveToPosition)
            {
                moveToPosition = true;
                SetRandomTarget();
            }
            else
            {
                state = State.Idle;
            }
        }

        float speedValue = navAgent.velocity.magnitude / navAgent.speed;
        animator.SetFloat("speed", speedValue);
    }

    private void MoveToTarget()
    {
        navAgent.isStopped = false;
        navAgent.speed = sprint_speed;
        navAgent.acceleration = sprint_speed * 2;
        navAgent.stoppingDistance = 1f;
        navAgent.SetDestination(target.position);

        float speedValue = navAgent.velocity.magnitude / navAgent.speed;
        animator.SetFloat("speed", speedValue);
    }

    public override void Attack()
    {
        if (IsDead) return;
        //if (!raycastHit) return;

        targetdistanceOnAttack = Vector3.Distance(transform.position, target.position);

        if (!hasAlternateAttack2Behaviour && targetdistanceOnAttack < attack2range)
        {
            if (!onAttack2)
            {
                navAgent.stoppingDistance = 1f;
                navAgent.isStopped = true;

                onAttack2 = true;
                animator.SetBool("OnAttack", false);
                animator.SetBool("onAttack2", true);
                animator.SetTrigger("attack2");
            }
        }
        else
        {
            navAgent.stoppingDistance = 2f;
            navAgent.isStopped = true;

            if (onAttack)
            {
                animator.SetBool("OnAttack", true);
            }
            else
            {
                animator.SetTrigger("attack");
                onAttack = true;
            }

            //shoot.CreateBulletInstanceEnemy(targetdistanceOnAttack + (targetdistanceOnAttack / targetdistanceOnAttackOffset));
        }
    }

    private void Attack2Alternative()
    {
        navAgent.isStopped = true;
        animator.SetTrigger("attack2");
        lastAttack2Alternative = Time.time;
    }

    private bool RayCastTargetCheck()
    {
        RaycastHit hit;

        if (Physics.Raycast(rayCastPos.position, rayCastPos.TransformDirection(Vector3.forward), out hit, lookRange, PlayerMask))
        {
            Debug.DrawRay(rayCastPos.position, rayCastPos.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            return true;
        }

        Debug.DrawRay(rayCastPos.position, rayCastPos.TransformDirection(Vector3.forward) * lookRange, Color.green);
        return false;

    }

    public override void LookAtTarget(Vector3 targetposition)
    {
        if (onChase)
        {
            targetposition.y = transform.position.y;
        }

        transform.LookAt(targetposition);
    }

    public override void LookAtTargetSmooth(Vector3 targetposition, float ratio)
    {
        if (onChase)
        {
            targetposition.y = transform.position.y;
        }

        base.LookAtTargetSmooth(targetposition, ratio);
    }

    public override void GetHit(float damage, DamageTypes.DamageType damageType)
    {
        animator.SetTrigger("takeDamage");
        base.GetHit(damage, damageType);
        Game.instance.PlayCrosshairEffect();

        if (!onChase)
        {
            state = State.Alert;
        }
    }

    public override void Death()
    {
        IsDead = true;        
        state = State.Death;
        manualState = true;
        onChase = false;
        onAttack = false;
        navAgent.isStopped = true;
        animator.SetBool("OnAttack", false);
        animator.SetBool("Dead", true);
        animator.SetTrigger("isDead");
        health.ToggleHealthBarVisibility(false);
        base.Death();
    }

    private void OnDrawGizmosSelected()
    {
        if (randomTargetTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(randomTargetTransform.position, 1f);           
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemySlime))]
public class DrawWireArc : Editor
{
    private void OnSceneGUI()
    {
        EnemySlime enemy = (EnemySlime)target;

        Handles.color = Color.red;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.AttackRange);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.LookRange);
        Handles.color = Color.green;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.MinDetectRange);
    }
}
#endif