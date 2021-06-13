using Elysium.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularAOEProjectileAttack : IAttack
{
    private GenericProjectile projectile = default;
    private float damageMultiplier = 1f;
    private int numOfProjectiles = 8;

    public float Range { get; set; }

    public CircularAOEProjectileAttack(float _range, GenericProjectile _projectile, float _damageMultiplier, int _numOfProjectiles)
    {
        this.Range = _range;
        this.projectile = _projectile;
        this.damageMultiplier = _damageMultiplier;
        this.numOfProjectiles = _numOfProjectiles;
    }

    public void Attack(IAttacker _ai, IDamageable _target)
    {
        Action<IDamageable> OnHit = (_target) =>
        {
            if (_target == null) { return; }
            if (_target.DamageableObject == _ai.DamageDealer.DamageDealerObject) { Debug.LogError("hit itself for some reason"); }
            _target.TakeDamage(_ai.DamageDealer, Mathf.CeilToInt(_ai.DamageDealer.Damage.Value * damageMultiplier));
        };

        for (int i = 0; i < numOfProjectiles; i++)
        {
            Vector3 dir = Vector2.down;
            dir = Quaternion.Euler(0, 0, (360/numOfProjectiles) * i) * dir;

            var proj = GameObject.Instantiate(projectile, _ai.Anim.Firepoint.position, projectile.transform.rotation);
            proj.Setup(dir.normalized, _ai.DamageDealer.DealsDamageToTeams, OnHit);
        }
    }
}
