using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointArea : MonoBehaviour
{
    [SerializeField] private Collider2D collider = default;

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        if (_collision.gameObject.TryGetComponent(out Player _player))
        {
            _player.Checkpoint = transform.position;
        }
    }

    private void OnValidate()
    {
        if (collider == null) { collider = GetComponent<Collider2D>(); }
        if (collider == null) { collider = gameObject.AddComponent<BoxCollider2D>(); }
        collider.isTrigger = true;
    }
}
