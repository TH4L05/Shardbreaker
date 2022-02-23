using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class EnemySlimeBoss : Enemy
{
    bool raycastHit;
    public bool isBoss;
    public bool onBossAttack;
    private bool alreadySpawned;
    public bool protectionPhase;
    private bool alreadyAttacked;
    public bool canSpawn;
    [SerializeField]
    private List<GameObject> spawner;
    [SerializeField]
    private GameObject slimePrefab;

    [SerializeField]
    private List<GameObject> crystals;

    public int crystalCount;
    public PlayableDirector shieldPlayable;
    public GameObject spikes;
    public GameObject ground;
    public GameObject shield;

    private BoxCollider bCollider;
    public static Action theBoss;

    private void FixedUpdate()
    {
        raycastHit = RayCastTargetCheck();
    }

    public override void StartSetup()
    {
        state = State.Idle;
        crystalCount = 1;
        bCollider = GetComponent<BoxCollider>();
    }

    public override void UpdateEnemy()
    {
        if (IsDead) return;

        if (protectionPhase)
        {
            state = State.ProtectPhase;
        }
        else
        {
            float distance = DistanceCheck();

            if (onBossAttack)
            {

            }
            else
            {
                if (!onChase)
                {
                    if (raycastHit && distance < lookRange)
                    {
                        state = State.Alert;

                        if (raycastHit && distance <= attackRange)
                        {
                            state = State.Attack;
                        }
                    }
                    else if (distance < minimumDectectRange)
                    {
                        state = State.Alert;
                    }
                }
                else
                {
                    if (distance <= attackRange)
                    {
                        if (raycastHit && !isBoss)
                        {
                            state = State.Attack;
                        }
                        else
                        {
                            state = State.Attack;
                        }
                    }
                    else
                    {
                        animator.SetBool("OnAttack", false);
                        onAttack = false;
                        state = State.MoveToTarget;
                    }
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
                    //LookAtTargetSmooth(target.position, 1.5f);
                    LookAtTarget(target.position);
                    onBossAttack = false;
                    MoveToTarget();
                    break;
                case State.MoveToPosition:
                    MoveToPosition();
                    break;
                case State.Attack:
                    //LookAtTargetSmooth(target.position, 2f);
                    LookAtTarget(target.position);
                    Attack();
                    break;
                case State.Death:
                    break;
                case State.BossAttack:
                    BossAttack();
                    break;
                case State.ProtectPhase:
                    ProtectionPhase();
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
        if (!onChase && !protectionPhase)
        {
            //LookAtTargetSmooth(target.position, 10f);
            AlertEnemiesInRange();
            animator.SetBool("alert", true);
            onChase = true;
            animator.SetBool("onChase", true);
            AlertEnemiesInRange();
            theBoss();
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
        navAgent.speed = 1f;
        navAgent.acceleration = 2f;
        navAgent.stoppingDistance = 0f;

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
        navAgent.speed = 4f;
        navAgent.acceleration = 8f;
        navAgent.stoppingDistance = 1f;
        navAgent.SetDestination(target.position);

        float speedValue = navAgent.velocity.magnitude / navAgent.speed;
        animator.SetFloat("speed", speedValue);
    }

    public override void Attack()
    {
        if (IsDead) return;
        //if (!raycastHit) return;

        navAgent.stoppingDistance = 2f;
        navAgent.isStopped = true;

        targetdistanceOnAttack = Vector3.Distance(transform.position, target.position);

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

    private void BossAttack()
    {
        onAttack = false;
        onChase = false;
        animator.SetBool("OnAttack", false);
        animator.SetTrigger("onAttack2");
        SpawnSlimes();
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
        if (protectionPhase) return;

        animator.SetTrigger("takeDamage");
        base.GetHit(damage, damageType);

        if (isBoss && !onBossAttack && !alreadyAttacked)
        {
            if (health.CurrentHealth == maxHealth / 2)
            {
                state = State.BossAttack;

                onBossAttack = true;
            }
            Debug.Log("Boss Health = " + health.CurrentHealth);
        }

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
    private void SpawnSlimes()
    {
        if (canSpawn)
        {
            if (!alreadySpawned)
            {
                foreach (GameObject Slime in spawner)
                {
                    CreateInstance(Slime.transform.position);
                }
                alreadySpawned = true;
            }
        }


    }

    private void CreateInstance(Vector3 position)
    {
        GameObject enemySlime = Instantiate(slimePrefab, position, Quaternion.identity);
        enemySlime.GetComponent<Enemy>().state = State.MoveToTarget;
    }

    private void ProtectionPhase()
    {
       
        if(crystalCount > 0)
        {
            onChase = false;
            onAttack = false;
            navAgent.isStopped = true;
            animator.SetBool("OnAttack", false);
            //bCollider.enabled = false;
        }
        else
        {
            animator.SetBool("onAttack2", false);
            //shield.SetActive(false);
            spikes.SetActive(false);
            // ground.SetActive(false);
            shieldPlayable.Play();
            StartCoroutine(WaitForPlayable());
            //bCollider.enabled = true;
            //make this a coroutine with delay to wait vfx playable
            alreadyAttacked = true;
            protectionPhase = false;
            //onChase = true;
            //state = State.MoveToTarget;
        }
    }

    IEnumerator WaitForPlayable()
    {
        yield return new WaitForSeconds(4f);
        shield.SetActive(false);
        onChase = true;
        state = State.MoveToTarget;
    }

    public void UpdateCrystalCount()
    {
        crystalCount--;
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
[CustomEditor(typeof(EnemySlimeBoss))]
public class DrawWireArcBoss : Editor
{
    private void OnSceneGUI()
    {
        EnemySlimeBoss enemy = (EnemySlimeBoss)target;

        Handles.color = Color.red;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.AttackRange);
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.LookRange);
        Handles.color = Color.green;
        Handles.DrawWireDisc(enemy.transform.position, new Vector3(0, 1, 0), enemy.MinDetectRange);
    }
}
#endif
