using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ethereal : MonoBehaviour
{
    private float maxDistance = 5f;

    private Movement movement = default;
    private Rigidbody2D rb = default;
    private Vector2? destination = null;

    public string Name { get; private set; }

    public void OnEnable()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnDisable()
    {
        
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
    }

    public void MoveToPosition(Vector2 _worldClickedPosition)
    {
        Vector2 direction = _worldClickedPosition - (Vector2)transform.position;
        destination =  (Vector2)transform.position + (direction.normalized * maxDistance);
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
