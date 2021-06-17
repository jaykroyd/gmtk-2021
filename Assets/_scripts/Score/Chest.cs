using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private float spawnRadius = 2f;
    [SerializeField] private RewardPackage reward = default;
    [SerializeField] private Reward rewardPrefab;

    [SerializeField, ReadOnly] private Rigidbody2D rb;
    [SerializeField, ReadOnly] private Collider2D collider;

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Reward");
    }

    private void OnCollisionStay2D(Collision2D _collision)
    {
        if (_collision.collider.TryGetComponent(out IPlayer _player))
        {
            Reward.Create(rewardPrefab, transform.position, reward, spawnRadius);
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        if (collider == null) 
        {
            collider = GetComponent<Collider2D>();
            if (collider == null)
            {
                Debug.LogError($"Chest {gameObject.name} doesn't have a collider. Attaching a default box collider.");
                collider = gameObject.AddComponent<BoxCollider2D>();
            }
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody2D>();
            }
        }
    }
}
