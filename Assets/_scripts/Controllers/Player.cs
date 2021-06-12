using Elysium.Combat;
using Elysium.Utils.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour, IPushable
{
    [SerializeField, ReadOnly] private Vector2 input = Vector2.zero;
    [SerializeField] private int health = 100;
    [SerializeField] private int damage = 10;
    [SerializeField] private Ethereal ethereal;
    [RequireInterface(typeof(IRangeIndicator))]
    [SerializeField] private UnityEngine.Object[] indicators = new UnityEngine.Object[0];        

    private Canvas canvas = default;
    private Movement movement = default;
    private Rigidbody2D rb = default;
    private HealthController healthController = default;

    bool isAiming = false;
    private Vector2? destination = null;

    private IEtherealEffect fireEffect = null;
    private IEtherealEffect waterEffect = null;
    private IEtherealEffect windEffect = null;
    private IEtherealEffect earthEffect = null;

    [Separator("Fire Effect", true)]
    [SerializeField] GameObject fireExplosionHit = null;
    [SerializeField] GameObject fireExplosionTick = null;

    [Separator("Vine Effect", true)]
    [SerializeField] GameObject vineLinkPrefab = null;
    [SerializeField] GameObject vineTopPrefab = null;

    private IEtherealEffect selectedEffect = null;

    public ModelController Anim { get; set; }
    public Rigidbody2D Rigidbody => rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        canvas = FindObjectOfType<Canvas>();
        movement = GetComponent<Movement>();
        healthController = GetComponent<HealthController>();
        Anim = GetComponent<ModelController>();

        fireEffect = new FireEffect(this, ethereal, Color.red, Color.red, 10f, 1f, fireExplosionHit, fireExplosionTick);
        waterEffect = new FireEffect(this, ethereal, Color.blue, Color.blue, 10f, 1f, fireExplosionHit, fireExplosionTick);
        windEffect = new WindEffect(this, ethereal, Color.yellow, Color.yellow, 0f, 5f, 200f);
        earthEffect = new VineEffect(this, ethereal, Color.green, Color.green, vineLinkPrefab, vineTopPrefab);

        healthController.MaxResource = new Elysium.Utils.RefValue<int>(() => health);
        healthController.Fill();
    }

    protected virtual void Start()
    {
        ethereal.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (destination.HasValue) { AutomaticallyMoveToDestination(); }
        else { MoveBasedOnInput(); }

        DrawChangeStateUI(isAiming);
        if (selectedEffect != null && Input.GetMouseButtonDown(0)) 
        {
            if (isAiming) { ShootEthereal(); }
            else if (ethereal.IsDeployed) { GoToEthereal(); }
        }
        
        if (selectedEffect != null && Input.GetMouseButtonDown(1)) 
        {
            if (isAiming) { DropEthereal(); }
            else if (ethereal.IsDeployed) { PullEthereal(); }
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha1))
        { 
            selectedEffect = fireEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha2)) 
        { 
            selectedEffect = waterEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha3))
        { 
            selectedEffect = windEffect;
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha4)) 
        { 
            selectedEffect = earthEffect;
            isAiming = true;
        }
    }

    private void AutomaticallyMoveToDestination()
    {
        Vector2 direction = destination.Value - (Vector2)transform.position;
        movement.Move(direction.normalized);

        if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
        {
            destination = null;
            rb.velocity = Vector2.zero;
            movement.MoveSpeed = 10f;
            ethereal.InvokePlayerArrival();
            return;
        }
    }

    private void MoveBasedOnInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = 0;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            y = 1;
        }

        input = new Vector2(x, y);
        Anim.SetMoveSpeed(input.x);
        var jFloat = movement.IsGrounded ? 0 : 1;
        Anim.SetAnimatorFloat("jump", jFloat);
        movement.Move(input.x, input.y == 1);
    }

    private void ShootEthereal()
    {
        ethereal.Shoot(this, selectedEffect);
        isAiming = false;
    }

    private void DropEthereal()
    {
        ethereal.Drop(this, selectedEffect);
        isAiming = false;
    }

    private void PullEthereal()
    {
        ethereal.Pull(this);
    }

    private void GoToEthereal()
    {
        destination = (Vector2)ethereal.transform.position;
        movement.MoveSpeed = 30f;
        ethereal.Goto(this);
    }

    private void DrawChangeStateUI(bool _active)
    {
        foreach (IRangeIndicator indicator in indicators)
        {
            indicator.Radius = 5f;
            indicator.SetActive(_active);
        }
    }

    public void Push(float _force, Vector2 _direction)
    {
        Debug.DrawRay(transform.position, _direction * _force, Color.red);
        Debug.LogError("pushed");
        rb.velocity = Vector2.zero;
        rb.AddForce(_direction * _force);
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.collider.TryGetComponent(out IDamageDealer _dealer))
        {
            Vector2 direction = (Vector2)ethereal.transform.position - (Vector2)_collision.collider.transform.position;
            healthController.TakeDamage(_dealer, _dealer.Damage.Value);
            Push(1000f, direction.normalized);
        }
    }
}
