using Elysium.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ethereal : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;
    [SerializeField] private ModelController[] models = new ModelController[4];

    private IEtherealEffect effect = default;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private Player player = default;    
    
    private float tickInterval = 1f;
    private float tickTimer = 0;

    public Transform Target { get; set; }
    public Vector2? Destination { get; set; }
    public SpriteRenderer Renderer { get; private set; }
    public SpectralLink Link { get; private set; }
    public ModelController Anim { get; set; }
    public float MaxDistance => maxDistance;

    public event UnityAction OnDestinationArrival;
    public event UnityAction OnPlayerArrival;

    public event UnityAction OnDeployStart;
    public event UnityAction OnDeployComplete;

    public event UnityAction OnRetrieveStart;
    public event UnityAction OnRetrieveComplete;

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

    public void Deploy(Player _controller, IEtherealEffect _effect)
    {
        this.effect = _effect;

        Activate();
        effect.DeployStart();
    }

    public void Retrieve(Player _controller)
    {
        effect.RetrieveStart();
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
        if (Destination.HasValue)
        {
            Vector2 direction = Destination.Value - (Vector2)transform.position;
            movement.Move(direction.normalized);

            if (Vector2.Distance((Vector2)transform.position, Destination.Value) < 0.5f)
            {
                Stop();
                OnDestinationArrival?.Invoke();
                return;
            }            
        }
        else if (Target != null)
        {
            Vector2 direction = (Vector2)Target.position - (Vector2)transform.position;
            movement.Move(direction.normalized);

            if (Vector2.Distance((Vector2)transform.position, (Vector2)Target.position) < 0.5f)
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
            if (Destination == null) { return; }
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
        Destination = null;
        Target = null;
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
        if (Destination.HasValue)
        {
            Gizmos.DrawWireSphere(Destination.Value, 1f);
        }
    }
}
