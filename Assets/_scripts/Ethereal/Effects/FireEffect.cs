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
    private bool isAttacking = false;

    public FireEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, float _collisionDamage, float _tickDamage) : base(_controller, _ethereal, _mainColor, _linkColor)
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

    public override void OnCollide(Collider2D _collider)
    {
        if (!isAttacking) { return; }
        TryDealDamage(_collider, collisionDamageMultiplier);
    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {
        TryDealDamage(_collider, tickDamageMultiplier);
    }    

    public override void OnDeactivate()
    {
        
    }

    public override void OnDrop()
    {
        
    }

    public override void OnGotoStart()
    {

    }

    public override void OnGotoEnd()
    {

    }

    public override void OnPullStart()
    {
        ethereal.Anim.PlayAnimation("Attack");
        isAttacking = true;
    }

    public override void OnPullEnd()
    {
        isAttacking = false;
    }

    public override void OnShootStart()
    {
        ethereal.Anim.PlayAnimation("Attack");
        isAttacking = true;
    }

    public override void OnShootEnd()
    {
        isAttacking = false;
    }

    private void TryDealDamage(Collider2D _collider, float _multiplier)
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
