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

    private IEtherealEffect fireEffect = null;
    private IEtherealEffect waterEffect = null;
    private IEtherealEffect windEffect = null;
    private IEtherealEffect earthEffect = null;

    [Separator("Hotbar", true)]
    [SerializeField] UI_HotbarSlot fireHotbar = null;
    [SerializeField] UI_HotbarSlot waterHotbar = null;
    [SerializeField] UI_HotbarSlot windHotbar = null;
    [SerializeField] UI_HotbarSlot earthHotbar = null;

    [Separator("Particles", true)]
    [SerializeField] GameObject[] particles = new GameObject[4];

    [Separator("Fire Effect", true)]
    [SerializeField] GameObject fireExplosionHit = null;
    [SerializeField] GameObject fireExplosionTick = null;

    [Separator("Vine Effect", true)]
    [SerializeField] GameObject vineLinkPrefab = null;
    [SerializeField] GameObject vineTopPrefab = null;

    private IEtherealEffect selectedEffect = null;

    public ModelController Anim { get; set; }
    public Rigidbody2D Rigidbody => rb;
    public Movement Movement => movement;
    public Vector2? Destination { get; set; }

    public void Push(float _force, Vector2 _direction)
    {
        Debug.DrawRay(transform.position, _direction * _force, Color.red);
        rb.velocity = Vector2.zero;
        rb.AddForce(_direction * _force);
    }

    public void SetParticles(int _index)
    {
        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(i == _index);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        canvas = FindObjectOfType<Canvas>();
        movement = GetComponent<Movement>();
        healthController = GetComponent<HealthController>();
        Anim = GetComponentInChildren<ModelController>();

        fireEffect = new FireEffect(
            this,
            ethereal, 
            Color.red, 
            Color.red, 
            0,
            Ethereal.BASE_TIME_IN_FORM,
            10f, 
            1f, 
            fireExplosionHit, 
            fireExplosionTick
            );

        waterEffect = new FireEffect(
            this, 
            ethereal, 
            Color.blue, 
            Color.blue, 
            1,
            Ethereal.BASE_TIME_IN_FORM,
            10f, 
            1f, 
            fireExplosionHit, 
            fireExplosionTick
            );

        windEffect = new WindEffect(
            this, 
            ethereal, 
            Color.white, 
            Color.white, 
            2,
            Ethereal.BASE_TIME_IN_FORM,
            0f, 
            5f,
            200f
            );
        
        earthEffect = new VineEffect(
            this, 
            ethereal, 
            Color.green, 
            Color.green, 
            3,
            Ethereal.BASE_TIME_IN_FORM,
            vineLinkPrefab, 
            vineTopPrefab
            );

        healthController.MaxResource = new Elysium.Utils.RefValue<int>(() => health);
        healthController.Fill();
    }

    protected virtual void Start()
    {
        ethereal.gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        if (Destination.HasValue) { AutomaticallyMoveToDestination(); }
        else { MoveBasedOnInput(); }

        DrawChangeStateUI(isAiming);
        if (selectedEffect != null && Input.GetMouseButtonDown(0)) 
        {
            if (isAiming) { DeployEthereal(); }
            else if (ethereal.IsDeployed) { RetrieveEthereal(); }
        }

        // Different Spirits
        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha1))
        { 
            selectedEffect = fireEffect;
            DeactivateAllHotbars();
            fireHotbar.Highlight(true);
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha2)) 
        { 
            selectedEffect = waterEffect;
            DeactivateAllHotbars();
            waterHotbar.Highlight(true);
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha3))
        { 
            selectedEffect = windEffect;
            DeactivateAllHotbars();
            windHotbar.Highlight(true);
            isAiming = true;
        }

        if (!ethereal.IsActive && Input.GetKeyDown(KeyCode.Alpha4)) 
        { 
            selectedEffect = earthEffect;
            DeactivateAllHotbars();
            earthHotbar.Highlight(true);
            isAiming = true;
        }
    }

    private void AutomaticallyMoveToDestination()
    {
        Vector2 direction = Destination.Value - (Vector2)transform.position;
        movement.Move(direction.normalized);

        if (Vector2.Distance((Vector2)transform.position, Destination.Value) < 0.5f)
        {
            Destination = null;
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

    private void DeployEthereal()
    {
        ethereal.Deploy(this, selectedEffect);
        isAiming = false;
        DeactivateAllHotbars();
    }

    private void RetrieveEthereal()
    {
        ethereal.Retrieve(this);
    }

    private void DrawChangeStateUI(bool _active)
    {
        foreach (IRangeIndicator indicator in indicators)
        {
            indicator.Radius = 5f;
            indicator.SetActive(_active);
        }
    }

    private void DeactivateAllHotbars()
    {
        fireHotbar.Highlight(false);
        waterHotbar.Highlight(false);
        windHotbar.Highlight(false);
        earthHotbar.Highlight(false);
    }    

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.collider.TryGetComponent(out IDamageDealer _dealer))
        {
            Vector2 direction = (Vector2)ethereal.transform.position - (Vector2)_collision.collider.transform.position;
            healthController.TakeDamage(_dealer, _dealer.Damage.Value);
            Push(1000f, direction.normalized);
            Anim.PlayAnimation("Hit");
        }
    }
}
