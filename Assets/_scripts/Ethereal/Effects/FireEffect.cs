using Elysium.Combat;
using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffect : BaseEffect, IEtherealEffect, IDamageDealer
{
    private float tickDamageMultiplier = 1f;
    private float collisionDamageMultiplier = 10f;

    public FireEffect(FSMController _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, float _collisionDamage, float _tickDamage) : base(_controller, _ethereal, _mainColor, _linkColor)
    {
        this.collisionDamageMultiplier = _collisionDamage;
        this.tickDamageMultiplier = _tickDamage;
    }

    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);

    public DamageTeam[] DealsDamageToTeams => new DamageTeam[] { DamageTeam.ENEMY };
    public GameObject DamageDealerObject => controller.gameObject;
    

    public void CriticalHit()
    {
        
    }

    public override void OnActivate()
    {
        mainColor.a = 0.3f;
        ethereal.Renderer.color = mainColor;
        ethereal.Link.SetColor(linkColor);
    }

    public override void OnCollide(Collider _collider)
    {
        TryDealDamage(_collider, collisionDamageMultiplier);
    }

    public override void OnLinkCollideTick(Collider _collider)
    {
        TryDealDamage(_collider, tickDamageMultiplier);
    }    

    public override void OnDeactivate()
    {
        
    }

    public override void OnDrop()
    {
        
    }

    public override void OnGoto()
    {
        
    }    

    public override void OnPull()
    {
        ethereal.Anim.PlayAnimation("Attack");
    }

    public override void OnShoot()
    {
        ethereal.Anim.PlayAnimation("Attack");
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
