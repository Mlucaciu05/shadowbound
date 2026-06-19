using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public GenericHealth health;
    public Animator animator;
    public Rigidbody enemyBody;
    public NavMeshAgent navMeshAgent;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float damage = 12f;
    public float defense = 5f;
    public float movementSpeed = 3.5f;
    public float attackCooldown = 1.4f;
    public float detectionRange = 14f;
    public float attackRange = 2f;
    public float knockbackForce = 2f;

    [Header("Movement")]
    public bool useNavMeshAgent = true;
    public bool rotateTowardTarget = true;

    [Header("Effects")]
    public ParticleSystem attackEffect;
    public ParticleSystem deathEffect;

    protected bool isDead;
    protected float nextAttackTime;
    private float currentMoveSpeed;
    private Coroutine slowRoutine;

    protected virtual void Awake()
    {
        if (health == null) health = GetComponent<GenericHealth>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (enemyBody == null) enemyBody = GetComponent<Rigidbody>();
        if (navMeshAgent == null) navMeshAgent = GetComponent<NavMeshAgent>();
    }

    protected virtual void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }

        currentMoveSpeed = movementSpeed;
        ConfigureHealth();

        if (navMeshAgent != null)
        {
            navMeshAgent.speed = movementSpeed;
            navMeshAgent.stoppingDistance = attackRange * 0.85f;
        }
    }

    protected virtual void Update()
    {
        if (isDead || target == null || health == null || health.IsDead) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > detectionRange)
        {
            SetMoving(false);
            return;
        }

        FaceTarget();

        if (distance <= attackRange)
        {
            SetMoving(false);
            TryAttack();
        }
        else
        {
            ChaseTarget();
        }
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        if (slowRoutine != null)
        {
            StopCoroutine(slowRoutine);
        }

        slowRoutine = StartCoroutine(SlowRoutine(slowPercent, duration));
    }

    protected virtual void ConfigureHealth()
    {
        if (health == null) return;

        health.maxHealth = Mathf.RoundToInt(maxHealth);
        health.currentHealth = health.maxHealth;
        health.defense = defense;
        health.onDeath.AddListener(HandleDeath);
    }

    protected virtual void ChaseTarget()
    {
        SetMoving(true);

        if (useNavMeshAgent && navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(target.position);
            return;
        }

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Vector3 move = direction.normalized * currentMoveSpeed * Time.deltaTime;
        if (enemyBody != null && !enemyBody.isKinematic)
        {
            enemyBody.MovePosition(enemyBody.position + move);
        }
        else
        {
            transform.position += move;
        }
    }

    protected virtual void TryAttack()
    {
        if (Time.time < nextAttackTime) return;

        nextAttackTime = Time.time + attackCooldown;
        SetTrigger("Attack");

        if (attackEffect != null)
        {
            CombatVFXManager.Spawn(attackEffect, transform.position + transform.forward + Vector3.up, transform.forward);
        }

        IDamageable damageable = target.GetComponentInParent<IDamageable>();
        if (damageable == null || damageable.IsDead) return;

        Vector3 hitDirection = target.position - transform.position;
        DamageData damageData = new DamageData(damage, DamageType.Physical, gameObject, knockbackForce, false)
            .WithHit(target.position + Vector3.up, hitDirection);

        damageable.TakeDamage(damageData);
    }

    protected virtual void HandleDeath()
    {
        isDead = true;
        SetTrigger("Die");

        if (deathEffect != null)
        {
            CombatVFXManager.Spawn(deathEffect, transform.position + Vector3.up, Vector3.up);
        }

        SetMoving(false);

        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider enemyCollider in colliders)
        {
            enemyCollider.enabled = false;
        }
    }

    protected void SetMoving(bool moving)
    {
        if (animator != null)
        {
            SetBool("IsMoving", moving);
            SetFloat("Speed", moving ? currentMoveSpeed : 0f);
        }

        if (!moving && navMeshAgent != null && navMeshAgent.enabled && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.isStopped = true;
        }
    }

    protected void FaceTarget()
    {
        if (!rotateTowardTarget || target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
    }

    protected void SetTrigger(string parameterName)
    {
        if (animator == null || !HasParameter(parameterName, AnimatorControllerParameterType.Trigger)) return;
        animator.SetTrigger(parameterName);
    }

    protected void SetBool(string parameterName, bool value)
    {
        if (animator == null || !HasParameter(parameterName, AnimatorControllerParameterType.Bool)) return;
        animator.SetBool(parameterName, value);
    }

    protected void SetFloat(string parameterName, float value)
    {
        if (animator == null || !HasParameter(parameterName, AnimatorControllerParameterType.Float)) return;
        animator.SetFloat(parameterName, value);
    }

    protected bool HasParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }

    private IEnumerator SlowRoutine(float slowPercent, float duration)
    {
        slowPercent = Mathf.Clamp01(slowPercent);
        currentMoveSpeed = movementSpeed * (1f - slowPercent);
        if (navMeshAgent != null) navMeshAgent.speed = currentMoveSpeed;

        yield return new WaitForSeconds(duration);

        currentMoveSpeed = movementSpeed;
        if (navMeshAgent != null) navMeshAgent.speed = currentMoveSpeed;
    }
}
