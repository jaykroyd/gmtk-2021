using Elysium.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ethereal : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private ModelController[] models = new ModelController[4];

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private Player player = default;

    private Vector2? destination = null;
    private Transform target = null;
    private float tickInterval = 1f;
    private float tickTimer = 0;

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
        player = FindObjectOfType<Player>();
    }

    public bool IsActive => gameObject.activeSelf;
    public bool IsDeployed { get; set; }

    public void Shoot(Player _controller, IEtherealEffect _effect)
    {
        this.effect = _effect;             
        transform.position = _controller.transform.position;

        var worldPos = Utils.GetCurrentMousePosition();
        worldPos = new Vector3(worldPos.x, worldPos.y, 0);
        Vector2 direction = worldPos - (Vector2)transform.position;
        destination = (Vector2)transform.position + (direction.normalized * maxDistance);

        Activate();
        effect.OnShootStart();

        void OnArriveAtDestination() 
        {
            IsDeployed = true; 
            OnDestinationArrival -= effect.OnShootEnd;
            OnDestinationArrival -= OnArriveAtDestination;            
        }

        OnDestinationArrival += effect.OnShootEnd;
        OnDestinationArrival += OnArriveAtDestination;        
    }

    public void Drop(Player _controller, IEtherealEffect _effect)
    {
        this.effect = _effect;
        transform.position = _controller.transform.position;

        Activate();
        effect.OnDrop();
        IsDeployed = true;
    }

    public void Pull(Player _controller)
    {
        target = _controller.transform;
        effect.OnPullStart();

        void DeactivateOnArrival()
        {
            OnPlayerArrival -= effect.OnPullEnd;
            OnPlayerArrival -= DeactivateOnArrival;
            Deactivate();
        }

        OnPlayerArrival += effect.OnPullEnd;
        OnPlayerArrival += DeactivateOnArrival;
    }

    public void Goto(Player _controller)
    {
        effect.OnGotoStart();

        void DeactivateOnArrival()
        {
            OnPlayerArrival -= DeactivateOnArrival;
            OnPlayerArrival -= effect.OnGotoEnd;
            Deactivate();
        }

        OnPlayerArrival += DeactivateOnArrival;
        OnPlayerArrival += effect.OnGotoEnd;
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
        
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            LinkTrigger();
            tickTimer = tickInterval;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        effect.OnCollide(other);

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {            
            if (destination == null) { return; }
            Stop();
            OnDestinationArrival?.Invoke();            
        }
    }

    private void LinkTrigger()
    {
        var hits = Physics2D.RaycastAll(transform.position, (player.transform.position - transform.position).normalized, (player.transform.position - transform.position).magnitude);
        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == player.gameObject || hit.collider.gameObject == this.gameObject) { continue; }
                // Debug.Log("Tick: " + hit.collider.name);
                effect.OnLinkCollideTick(hit.collider);
            }
        }
    }

    private void Stop()
    {
        destination = null;
        target = null;
        rb.velocity = Vector2.zero;        
    }

    public void SetModel(int _index)
    {
        foreach (var model in models)
        {
            model.gameObject.SetActive(false);
        }

        if (_index >= models.Length) 
        {
            Debug.LogError("index out of bounds of array");
            return; 
        }

        models[_index].gameObject.SetActive(true);
        Anim = models[_index];
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
