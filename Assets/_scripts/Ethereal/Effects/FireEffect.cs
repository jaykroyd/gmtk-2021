using Elysium.Combat;
using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffect : IEtherealEffect, IDamageDealer
{
    private Ethereal ethereal = default;
    private FSMController controller = default;

    private float tickDamageMultiplier = 1f;
    private float collisionDamageMultiplier = 10f;

    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);

    public DamageTeam[] DealsDamageToTeams => new DamageTeam[] { DamageTeam.ENEMY };
    public GameObject DamageDealerObject => controller.gameObject;

    public FireEffect(FSMController _controller, Ethereal _ethereal, float _collisionDamage, float _tickDamage)
    {
        this.collisionDamageMultiplier = _collisionDamage;
        this.tickDamageMultiplier = _tickDamage;
        controller = _controller;
        ethereal = _ethereal;
    }

    public void CriticalHit()
    {
        
    }

    public void OnActivate()
    {
        
    }

    public void OnCollide(Collider _collider)
    {
        TryDealDamage(_collider, collisionDamageMultiplier);
    }

    public void OnLinkCollideTick(Collider _collider)
    {
        TryDealDamage(_collider, tickDamageMultiplier);
    }    

    public void OnDeactivate()
    {
        
    }

    public void OnDrop()
    {
        
    }

    public void OnGoto()
    {
        
    }    

    public void OnPull()
    {
        
    }

    public void OnShoot()
    {        

    }

    private void TryDealDamage(Collider _collider, float _multiplier)
    {
        if (_collider.TryGetComponent(out IDamageable _unit))
        {
            if (DealsDamageToTeams.Contains(_unit.Team))
            {
                _unit.TakeDamage(this, Mathf.CeilToInt(Damage.Value * _multiplier), "Fire");
            }
        }
    }
}
