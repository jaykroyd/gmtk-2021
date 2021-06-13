using Elysium.Combat;
using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireEffect : BaseEffect, IDamageDealer
{
    private float tickDamageMultiplier = 1f;
    private float collisionDamageMultiplier = 10f;
    private bool isAttacking = false;
    private GameObject hitExplosionEffect = default;
    private GameObject tickExplosionEffect = default;

    public FireEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _cooldown, float _collisionDamage, float _tickDamage, GameObject _hitExplosionEffect, GameObject _tickExplosionEffect) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm, _cooldown)
    {
        this.collisionDamageMultiplier = _collisionDamage;
        this.tickDamageMultiplier = _tickDamage;
        this.hitExplosionEffect = _hitExplosionEffect;
        this.tickExplosionEffect = _tickExplosionEffect;
    }

    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);

    public DamageTeam[] DealsDamageToTeams => new DamageTeam[] { DamageTeam.ENEMY };
    public GameObject DamageDealerObject => controller.gameObject;
    

    public void CriticalHit()
    {
        
    }

    public override void OnActivate()
    {
        base.OnActivate();        
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    public override void OnCollide(Collider2D _collider)
    {
        if (!isAttacking) { return; }
        TryDealDamage(_collider, collisionDamageMultiplier, hitExplosionEffect);
    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {
        TryDealDamage(_collider, tickDamageMultiplier, tickExplosionEffect);
    }    

    public override void DeployStart()
    {
        Shoot();
        ethereal.Anim.PlayAnimation("Attack");
        isAttacking = true;
    }

    public override void DeployFinish()
    {
        isAttacking = false;
    }

    public override void RetrieveStart()
    {
        Pull();
        ethereal.Anim.PlayAnimation("Attack");
        isAttacking = true;
    }

    public override void RetrieveFinish()
    {
        isAttacking = false;
    }

    private void TryDealDamage(Collider2D _collider, float _multiplier, GameObject _effect)
    {        
        if (_collider.TryGetComponent(out IDamageable _unit))
        {
            if (DealsDamageToTeams.Contains(_unit.Team))
            {
                _unit.TakeDamage(this, Mathf.CeilToInt(Damage.Value * _multiplier), "Fire");
                GameObject.Instantiate(_effect, _collider.transform);
            }
        }
    }    
}
