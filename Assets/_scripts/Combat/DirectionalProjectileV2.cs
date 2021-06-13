using Elysium.Combat;
using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DirectionalProjectileV2 : GenericProjectile
{
    [SerializeField] private GameObject explosion = default;

    private void Start()
    {
        transform.LookAt((Vector2)origin + (Vector2)direction.Value);
    }

    public override void Move()
    {
        transform.Translate(transform.InverseTransformDirection(transform.forward) * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Platforms"))
        {
            GameObject.Instantiate(explosion, transform.position, explosion.transform.rotation);
            OnHitTarget(null);
        }

        var damageable = other.gameObject.GetComponentInChildren<IDamageable>();
        if (damageable == null) { return; }
        if (!dealsDamageTo.Contains(damageable.Team)) { return; }

        // VALID TARGET
        OnHitTarget(damageable);
        GameObject.Instantiate(explosion, damageable.DamageableObject.transform);
    }
}
