using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ethereal : MonoBehaviour
{
    [SerializeField] private float maxDistance = 5f;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private Vector2? destination = null;
    private Transform target = null;

    private IEtherealEffect effect = default;

    public string Name { get; private set; }

    public void Awake()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot()
    {
        effect.OnShoot();
    }

    public void Drop()
    {
        effect.OnDrop();
    }

    public void Pull()
    {
        effect.OnPull();
    }

    public void Goto()
    {
        effect.OnGoto();
    }

    private void Activate()
    {
        effect.OnActivate();
    }

    public void Deactivate()
    {
        effect.OnDeactivate();
    }

    public void Update()
    {
        if (destination.HasValue)
        {
            if (Vector2.Distance((Vector2)transform.position, destination.Value) < 0.5f)
            {
                Stop();
                return;
            }

            Vector2 direction = destination.Value - (Vector2)transform.position;
            movement.Move(direction.normalized);
        }
        else if (target != null)
        {
            if (Vector2.Distance((Vector2)transform.position, (Vector2)target.position) < 0.5f)
            {
                Stop();
                gameObject.SetActive(false);
                return;
            }

            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            movement.Move(direction.normalized);
        }
    }

    public void SetDirection(Vector2 _worldClickedPosition)
    {
        Vector2 direction = _worldClickedPosition - (Vector2)transform.position;
        destination =  (Vector2)transform.position + (direction.normalized * maxDistance);
    }

    public void SetDestination(Transform _destination)
    {
        target = _destination;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Stop();
        }
    }

    private void Stop()
    {
        Debug.Log("arrived at destination");
        destination = null;
        target = null;
        rb.velocity = Vector2.zero;
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
