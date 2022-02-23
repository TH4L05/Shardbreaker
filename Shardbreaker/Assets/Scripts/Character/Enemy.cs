using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    public enum State
    {
        Idle,
        MoveToTarget,
        MoveToPosition,
        MoveToAttackPosition,
        Attack,
        Death,
        Alert,
        BossAttack,
        ProtectPhase
    }

    [Header("Enemy Spezific")]
    [SerializeField] protected NavMeshAgent navAgent;
    [SerializeField] protected Shoot shoot;
    [SerializeField] protected float attackRange = 3.0f;
    [SerializeField] protected float lookRange = 10f;
    [SerializeField] protected float minimumDectectRange = 4f;
    [SerializeField] protected float idleTime = 3.0f;
    [SerializeField] protected Transform randomTargetTransform;
    public Transform target;
    [SerializeField] protected LayerMask PlayerMask;
    [SerializeField] protected LayerMask EnemyMask;

    [Header("Audio")]
    public AkAmbient moveAudio;
    public AkAmbient attackAudio;
    public AkAmbient attack2Audio;
    public AkAmbient dieAudio;


    [Header("VFX")]
    [SerializeField] protected GameObject deathVFX;
    [SerializeField] protected GameObject orbTemplate;

    [Header("Other")]
    public bool manualState;
    public State state;
    public Transform rayCastPos;
    public bool onChase;
    protected float idleTimeCurrent = 0f;
    private bool onDestroy;
    protected bool onAttack;
    public bool moveToPosition;
    public float targetdistanceOnAttack;
    public float targetdistanceOnAttackOffset = 3f;

    public static Action EnemyIsDead;
    public static Action EnemyStartAttack;

    public bool IsDead { get; set; }

    public float AttackRange => attackRange;
    public float LookRange => lookRange;
    public float MinDetectRange => minimumDectectRange;


    private void Awake()
    {
        IsDead = false;
        randomTargetTransform = GameObject.Find("RandomTargetPosition").transform;
        target = GameObject.Find("Player").transform;
        timer = GetComponent<Timer>();
    }

    public override void StartSetup()
    {
        SpecialSetup();
    }

    public virtual void SpecialSetup()
    {
    }

    void Update()
    {
        if (IsDead && !timer.Timer_isRunning)
        {
            if (onDestroy) return;
            onDestroy = true;
            if (deathVFX)
            {
                Instantiate(deathVFX, transform.position, transform.rotation);
            }           
            DeathSetUp();
            Destroy();
        }
        else
        {
            UpdateEnemy();
        }
    }


    public virtual void UpdateEnemy()
    {
    }

    public virtual void CheckState()
    {
    }

    public virtual void LookAtTarget(Vector3 targetposition)
    {
        transform.LookAt(targetposition);
    }

    public virtual void LookAtTargetSmooth(Vector3 targetposition, float ratio)
    {
        Vector3 direction = (targetposition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * ratio);
    }

    protected Vector3 RandomTargetPosition()
    {
        Vector3 position = UnityEngine.Random.insideUnitSphere * 5;
        position += transform.position;

        NavMesh.SamplePosition(position, out NavMeshHit hit, 20, 1);

        return hit.position;
    }

    protected void SetRandomTarget()
    {
        randomTargetTransform.position = RandomTargetPosition();
        navAgent.SetDestination(randomTargetTransform.position);
    }

    void DeathSetUp()
    {
        EnemyIsDead();
        if (orbTemplate != null)
        {
            Instantiate(orbTemplate, transform.position, Quaternion.identity);
        }
    }
}
