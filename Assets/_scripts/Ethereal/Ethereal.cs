using Elysium.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ethereal : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;

    private Movement movement = default;
    private Rigidbody2D rb = default;    

    private Vector2? destination = null;
    private Transform target = null;

    private IEtherealEffect effect = default;

    public SpriteRenderer Renderer { get; private set; }
    public SpectralLink Link { get; private set; }
    public ModelController Anim { get; set; }

    public event UnityAction OnDestinationArrival;
    public event UnityAction OnPlayerArrival;

    public void Awake()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        Renderer = GetComponent<SpriteRenderer>();
        Link = GetComponentInChildren<SpectralLink>();
        Anim = GetComponent<ModelController>();
    }

    public bool IsActive => gameObject.activeSelf;
    public bool IsDeployed { get; set; }

    public void Shoot(FSMController _controller, IEtherealEffect _effect)
    {
        this.effect = _effect;             
        transform.position = _controller.transform.position;

        var worldPos = Utils.GetCurrentMousePosition();
        worldPos = new Vector3(worldPos.x, worldPos.y, 0);
        Vector2 direction = worldPos - (Vector2)transform.position;
        destination = (Vector2)transform.position + (direction.normalized * maxDistance);

        Activate();
        effect.OnShoot();

        void OnArriveAtDestination() 
        {
            IsDeployed = true;
            OnDestinationArrival -= OnArriveAtDestination;            
        }

        OnDestinationArrival += OnArriveAtDestination;
    }

    public void Drop(FSMController _controller, IEtherealEffect _effect)
    {
        this.effect = _effect;
        transform.position = _controller.transform.position;

        Activate();
        effect.OnDrop();
        IsDeployed = true;
    }

    public void Pull(FSMController _controller)
    {
        target = _controller.transform;
        effect.OnPull();
        OnPlayerArrival += DeactivateOnArrival;
    }

    public void Goto(FSMController _controller)
    {
        effect.OnGoto();
        OnPlayerArrival += DeactivateOnArrival;
    }

    private void Activate()
    {
        this.gameObject.SetActive(true);
        effect.OnActivate();
    }

    public void Deactivate()
    {
        IsDeployed = false;
        this.gameObject.SetActive(false);
        effect.OnDeactivate();
    }

    private void DeactivateOnArrival()
    {
        OnPlayerArrival -= DeactivateOnArrival;
        Deactivate();        
    }

    private void Update()
    {
        if (destination.HasValue)
        {
            Vector2 direction = destination.Value - (Vector2)transform.position;
            movement.Move(direction.normalized);

            if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
            {
                Stop();
                OnDestinationArrival?.Invoke();
                return;
            }            
        }
        else if (target != null)
        {
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            movement.Move(direction.normalized);

            if (Vector2.Distance((Vector2)transform.position, (Vector2)target.position) < 0.5f)
            {
                Stop();
                OnPlayerArrival?.Invoke();
                return;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            if (destination == null) { return; }                
            Debug.Log("Moo");
            Stop();
            OnDestinationArrival?.Invoke();
        }
    }

    private void Stop()
    {
        destination = null;
        target = null;
        rb.velocity = Vector2.zero;        
    }

    public void InvokePlayerArrival()
    {
        OnPlayerArrival?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        if (destination.HasValue)
        {
            Gizmos.DrawWireSphere(destination.Value, 1f);
        }
    }
}
