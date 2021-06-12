using Elysium.Combat;
using Elysium.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaterEffect : BaseEffect, IDamageDealer
{
    private float healingTickMultiplier = 1f;
    private GameObject healEffectTick = null;

    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 1);
    public DamageTeam[] DealsDamageToTeams => new DamageTeam[] { DamageTeam.ENEMY };
    public GameObject DamageDealerObject => controller.gameObject;

    public WaterEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _healingTickMultiplier, GameObject _healEffectTick) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm)
    {
        this.healingTickMultiplier = _healingTickMultiplier;
        this.healEffectTick = _healEffectTick;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        controller.ActivateCollisions(true);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        controller.ActivateCollisions(false);
    }

    public override void OnCollide(Collider2D _collider)
    {

    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {
        if (_collider.TryGetComponent(out IDamageable _unit))
        {
            if (DealsDamageToTeams.Contains(_unit.Team))
            {
                GameObject.Instantiate(healEffectTick, controller.transform);
                controller.HealthController.Heal(this, Mathf.CeilToInt(Damage.Value * healingTickMultiplier), "Water");
            }
        }        
    }

    public override void DeployStart()
    {
        Drop();
    }

    public override void DeployFinish()
    {
        
    }    

    public override void RetrieveStart()
    {
        GoTo();
    }

    public override void RetrieveFinish()
    {
        
    }

    public void CriticalHit()
    {
        throw new System.NotImplementedException();
    }
}
