using Elysium.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : IAttack
{
    private GenericProjectile projectile = default;
    private float damageMultiplier = 1f;

    public float Range { get; set; }

    public ProjectileAttack(float _range, GenericProjectile _projectile, float _damageMultiplier)
    {
        this.Range = _range;
        this.projectile = _projectile;
        this.damageMultiplier = _damageMultiplier;
    }    

    public void Attack(IAttacker _ai, IDamageable _target)
    {
        Action<IDamageable> OnHit = (_target) => 
        { 
            if (_target.DamageableObject == _ai.DamageDealer.DamageDealerObject) { Debug.LogError("hit itself for some reason"); }
            _target.TakeDamage(_ai.DamageDealer, Mathf.CeilToInt(_ai.DamageDealer.Damage.Value * damageMultiplier)); 
        };

        var proj = GameObject.Instantiate(projectile, _ai.Anim.Firepoint.position, projectile.transform.rotation);
        proj.Setup(_target, _ai.DamageDealer.DealsDamageToTeams, OnHit);
    }
}
