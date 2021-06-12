using Elysium.Combat;
using Elysium.Utils;
using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    [SerializeField, ReadOnly] private Vector2 input = Vector2.zero;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private float wanderCooldown = 2f;
    [SerializeField] private Transform patrolA, patrolB = default;

    private Vector2? destination = null;
    private float wanderTimer = 0f;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private HealthController healthController = default;
    
    public ModelController Anim { get; set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<Movement>();
        healthController = GetComponent<HealthController>();
        Anim = GetComponent<ModelController>();

        wanderTimer = wanderCooldown;

        healthController.MaxResource = new Elysium.Utils.RefValue<int>(() => health);
        healthController.OnDeath += Die;
        healthController.Fill();
    }

    private void Update()
    {
        if (healthController.IsDead) { return; }

        if (destination.HasValue)
        {
            Vector2 direction = destination.Value - (Vector2)transform.position;
            input.x = Mathf.Clamp(direction.x, -1, 1);
            input.y = Mathf.Clamp(direction.y, 0, 1);

            if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
            {
                destination = null;
                rb.velocity = Vector2.zero;                
                return;
            }
        }
        else
        {
            input = Vector2.zero;
            wanderTimer -= Time.deltaTime;
            if (wanderTimer <= 0)
            {
                destination = RandomPatrolPoint();
                wanderTimer = wanderCooldown;
            }            
        }

        MoveBasedOnInput();
    }

    private void MoveBasedOnInput()
    {        
        Anim.SetMoveSpeed(input.x);
        var jFloat = movement.IsGrounded ? 0 : 1;
        Anim.SetAnimatorFloat("jump", jFloat);
        movement.Move(input.x, input.y == 1);
    }

    private void Die()
    {
        Destroy(gameObject, 1f);
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
}
