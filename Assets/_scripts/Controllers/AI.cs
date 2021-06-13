using Elysium.Combat;
using Elysium.Utils;
using Elysium.Utils.Attributes;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI : MonoBehaviour, IPushable, IDamageDealer, IAttacker
{
    [SerializeField, ReadOnly] private Vector2 input = Vector2.zero;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float wanderCooldown = 2f;
    [SerializeField] private float aggroRange = 8f;
    [SerializeField] private float maxAggroRange = 10f;    
    [SerializeField] private Transform patrolA, patrolB = default;
    [SerializeField] private RewardPackage reward = default;
    [SerializeField] private Reward rewardPrefab = default;    

    private Vector2? destination = null;
    private IDamageable target = null;

    private float wanderTimer = 0f;
    private float jumpCooldown = 2f;
    float raycastDistance = 3f;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private HealthController healthController = default;
    private Player player = default;
    private TimerInstance jumpTimer = default;

    public IModelController Anim { get; set; }
    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);

    public DamageTeam[] DealsDamageToTeams => new DamageTeam[]{ DamageTeam.PLAYER };
    public GameObject DamageDealerObject => gameObject;
    public IDamageDealer DamageDealer => this;

    private IAttack attack = default;    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        healthController = GetComponentInChildren<HealthController>();
        Anim = GetComponent<IModelController>();
        player = FindObjectOfType<Player>();

        wanderTimer = wanderCooldown;
        jumpTimer = Timer.CreateEmptyTimer(() => !this, true);

        Damage = new RefValue<int>(() => damage);

        healthController.MaxResource = new RefValue<int>(() => health);
        healthController.OnDeath += Die;
        healthController.Fill();

        attack = new MeleeAttack();
    }

    private void Update()
    {
        if (healthController.IsDead) { return; }

        if (target == null && EnemyIsInRange(out IDamageable _enemy))
        {
            target = _enemy;
        }

        if (target != null)
        {
            if (target.IsDead || target.DamageableObject == null)
            {
                target = null;
                input = Vector2.zero;
            }
            else
            {
                var tTransform = target.DamageableObject.transform;

                // Set Inputs
                Vector2 direction = (Vector2)tTransform.position - (Vector2)transform.position;
                input.x = Mathf.Clamp(direction.x, -1, 1);
                SetInputY(direction);                

                if (Vector2.Distance((Vector2)transform.position, (Vector2)tTransform.position) < attack.Range)
                {
                    attack.Attack(this, target);
                    rb.velocity = Vector2.zero;
                    input = Vector2.zero;
                }
                else if (Vector2.Distance((Vector2)transform.position, (Vector2)tTransform.position) > maxAggroRange)
                {
                    target = null;
                    rb.velocity = Vector2.zero;
                    input = Vector2.zero;
                }
            }            
        }
        else if (destination.HasValue)
        {
            // Set Inputs
            Vector2 direction = destination.Value - (Vector2)transform.position;
            input.x = Mathf.Clamp(direction.x, -1, 1);
            SetInputY(direction);

            if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
            {
                destination = null;
                rb.velocity = Vector2.zero;
                input = Vector2.zero;
            }
        }
        else 
        {
            WaitAndAcquireNewPatrolPosition(); 
        }

        MoveBasedOnInput();
    }

    private void SetInputY(Vector2 _direction)
    {
        if (jumpTimer.IsEnded)
        {
            Debug.DrawRay(transform.position, new Vector2(_direction.x, 0).normalized * raycastDistance, Color.red);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, new Vector2(_direction.x, 0).normalized, raycastDistance);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    bool isInLayer = movement.WhatIsGround.value == (movement.WhatIsGround.value | (1 << hit.collider.gameObject.layer));
                    if (isInLayer)
                    {
                        jumpTimer.SetTime(jumpCooldown);
                        input.y = 1;
                        return;
                    }
                }                
            }
            
            if (_direction.y >= 1)
            {
                jumpTimer.SetTime(jumpCooldown);
                input.y = Mathf.Clamp(_direction.y, 0, 1);
                return;
            }
        }

        input.y = 0;
    }

    private bool EnemyIsInRange(out IDamageable _damageable)
    {
        _damageable = null;
        if (player == null) { return false; }

        if (Vector2.Distance(player.transform.position, transform.position) < aggroRange)
        {
            _damageable = player.GetComponent<IDamageable>();
            return true;
        }
        
        return false;
    }

    private void WaitAndAcquireNewPatrolPosition()
    {
        input = Vector2.zero;
        wanderTimer -= Time.deltaTime;
        if (wanderTimer <= 0)
        {
            destination = RandomPatrolPoint();
            wanderTimer = wanderCooldown;
        }
    }

    private void MoveBasedOnInput()
    {        
        (Anim as ModelController).SetMoveSpeed(input.x);
        var jFloat = movement.IsGrounded ? 0 : 1;
        (Anim as ModelController).SetAnimatorFloat("jump", jFloat);
        movement.Move(input.x, input.y == 1);
    }

    private void Die()
    {
        Anim.PlayAnimation("Death");
        DropScore();
        Destroy(gameObject, 1f);
    }

    private void DropScore()
    {
        Reward.Create(rewardPrefab, transform.position, reward);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (patrolA != null) { Gizmos.DrawWireSphere(patrolA.position, 1f); }
        if (patrolB != null) { Gizmos.DrawWireSphere(patrolB.position, 1f); }        
    }

    private Vector2 RandomPatrolPoint()
    {
        float rx = Random.Range(patrolA.position.x, patrolB.position.x);
        float ry = Random.Range(patrolA.position.y, patrolB.position.y);
        return new Vector2(rx, ry);
    }

    public void Push(float _force, Vector2 _direction)
    {
        rb.AddForce(_direction * _force);
    }

    public void CriticalHit()
    {
        
    }
}
