using Elysium.Combat;
using Elysium.Utils;
using Elysium.Utils.Attributes;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boss : MonoBehaviour, IPushable, IDamageDealer, IAttacker
{
    private const float INITIAL_ATTACK_TIMER_DELAY = 0f;

    [SerializeField, ReadOnly] private Vector2 input = Vector2.zero;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float attackInterval = 5f;
    [SerializeField] private RewardPackage reward = default;
    [SerializeField] private Reward rewardPrefab = default;

    [Separator("Attacks", true)]
    [SerializeField] private FireGroundArea fireGround = default;
    [SerializeField] private GenericProjectile projectileTargetted = default;
    [SerializeField] private GenericProjectile projectileDirectional = default;

    private Vector2? destination = null;
    private IDamageable target = null;
    private IAttack[] attacks = default;

    private float jumpCooldown = 2f;
    float raycastDistance = 3f;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private HealthController healthController = default;
    private Player player = default;
    private TimerInstance jumpTimer = default;
    private TimerInstance attackTimer = default;

    public IModelController Anim { get; set; }
    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);
    public IDamageDealer DamageDealer => this;

    public DamageTeam[] DealsDamageToTeams => new DamageTeam[] { DamageTeam.PLAYER };
    public GameObject DamageDealerObject => gameObject;

    private IAttack PreviousAttack { get; set; }
    private IAttack SelectedAttack { get; set; }   

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        healthController = GetComponentInChildren<HealthController>();
        Anim = GetComponentInChildren<IModelController>();
        player = FindObjectOfType<Player>();
        target = player.GetComponentInChildren<IDamageable>();

        jumpTimer = Timer.CreateEmptyTimer(() => !this, true);
        attackTimer = Timer.CreateTimer(INITIAL_ATTACK_TIMER_DELAY, () => !this, true);

        Damage = new RefValue<int>(() => damage);

        healthController.MaxResource = new RefValue<int>(() => health);
        healthController.OnDeath += Die;
        healthController.Fill();

        attacks = new IAttack[]
        {
            new FireGroundAttack(fireGround, 7f, 1f),
            // new ProjectileAttack(Mathf.Infinity, projectileTargetted, 1f),
            new DirectionalProjectileAttack(Mathf.Infinity, projectileDirectional, 1f),
            new CircularAOEProjectileAttack(Mathf.Infinity, projectileDirectional, 1f, 16),
        };
    }

    private void Update()
    {
        if (healthController.IsDead || target == null) { return; }

        if (SelectedAttack == null) { PickNextAttack(); }

        if (SelectedAttack != null)
        {
            SelectedAttack.Attack(this, target);
            PreviousAttack = SelectedAttack;
            SelectedAttack = null;
            input = Vector2.zero;
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
            WaitAndAcquireNewPosition();
        }

        MoveBasedOnInput();
    }

    private void PickNextAttack()
    {
        if (!attackTimer.IsEnded) { return; }

        if (attacks.Length < 1)
        {
            // Debug.LogError($"No attacks setup");
            return;
        }

        List<IAttack> availableAttacks = new List<IAttack>(attacks);
        if (PreviousAttack != null) { availableAttacks.Remove(PreviousAttack); }
        if (availableAttacks.Count < 1) 
        {
            // Debug.LogError($"No available attacks");
            return; 
        }

        int r = Random.Range(0, availableAttacks.Count);        
        SelectedAttack = availableAttacks[r];
        attackTimer.SetTime(attackInterval);

        Debug.Log($"Selected new attack: "+ SelectedAttack);
    }

    private void WaitAndAcquireNewPosition()
    {
        
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

    private void MoveBasedOnInput()
    {
        (Anim as ModelController).SetMoveSpeed(input.x);
        var jFloat = movement.IsGrounded ? 0 : 1;
        (Anim as ModelController).SetAnimatorFloat("jump", jFloat);
        movement.Move(input.x, input.y == 1);
    }

    private void Die()
    {
        DropScore();
        Destroy(gameObject, 1f);
    }

    private void DropScore()
    {
        Reward.Create(rewardPrefab, transform.position, reward);
    }

    public void Push(float _force, Vector2 _direction)
    {
        rb.AddForce(_direction * _force);
    }

    public void CriticalHit()
    {

    }
}
