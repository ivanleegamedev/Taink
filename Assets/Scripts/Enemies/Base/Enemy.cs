using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable, IMoveable, ITriggerCheckable
{
    #region Variables
    [field: SerializeField] public float MaxHealth { get; set; } = 100.0f;
    public float CurrentHealth { get; set; }
    public Rigidbody rb { get; set; }
    public bool IsAggroed { get; set; }
    public bool IsWithinAttackDistance { get; set; }
    #endregion



    #region State Machine Variables
    public EnemyStateMachine StateMachine { get; set; }
    public EnemyIdleState IdleState { get; set; }
    public EnemyChaseState ChaseState { get; set; }
    public EnemyAttackState AttackState { get; set; }
    #endregion



    #region ScriptableObject Variables
    [SerializeField] private SO_Base_EnemyIdle EnemyIdleBase;
    [SerializeField] private SO_Base_EnemyChase EnemyChaseBase;
    [SerializeField] private SO_Base_EnemyAttack EnemyAttackBase;

    public SO_Base_EnemyIdle EnemyIdleBaseInstance { get; set; }
    public SO_Base_EnemyChase EnemyChaseBaseInstance { get; set; }
    public SO_Base_EnemyAttack EnemyAttackBaseInstance { get; set; }
    #endregion



    #region Main Methods
    private void Awake()
    {
        EnemyIdleBaseInstance = Instantiate(EnemyIdleBase);
        EnemyChaseBaseInstance = Instantiate(EnemyChaseBase);
        EnemyAttackBaseInstance = Instantiate(EnemyAttackBase);

        StateMachine = new EnemyStateMachine();

        IdleState = new EnemyIdleState(this, StateMachine);
        ChaseState = new EnemyChaseState(this, StateMachine);
        AttackState = new EnemyAttackState(this, StateMachine);
    }

    private void Start()
    {
        CurrentHealth = MaxHealth;

        rb = GetComponent<Rigidbody>();

        EnemyIdleBaseInstance.Initialize(gameObject, this);
        EnemyChaseBaseInstance.Initialize(gameObject, this);
        EnemyAttackBaseInstance.Initialize(gameObject, this);

        StateMachine.Initialize(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentEnemyState.FrameUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentEnemyState.PhysicsUpdate();
    }
    #endregion



    #region Health and Damage Methods
    public void Damage(float damageAmount)
    {
        CurrentHealth -= damageAmount;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    #endregion



    #region Movement Functions
    public void MoveEnemy(Vector2 velocity)
    {
        rb.velocity = velocity;
    }
    #endregion



    #region Distance and Aggro Methods
    public void SetAggroStatus(bool isAggroed)
    {
        IsAggroed = isAggroed;
    }

    public void SetAttackDistanceStatus(bool isWithinAttackDistance)
    {
        IsWithinAttackDistance = isWithinAttackDistance;
    }
    #endregion



    #region Animation Triggers
    private void AnimationTriggerEvent(AnimationTriggerType triggerType)
    {
        StateMachine.CurrentEnemyState.AnimationTriggerEvent(triggerType);
    }

    public enum AnimationTriggerType
    {
        EnemyDamaged,
        PlayMovementSound,
    }
    #endregion
}