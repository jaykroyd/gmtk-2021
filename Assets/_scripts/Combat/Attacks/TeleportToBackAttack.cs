using Elysium.Combat;
using Elysium.Utils.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToBackAttack : IAttack
{
    private GameObject poofEffect = default;

    public float Range { get; set; }

    public TeleportToBackAttack(float _range, GameObject _poofEffect)
    {
        this.Range = _range;
        this.poofEffect = _poofEffect;
    }

    public void Attack(IAttacker _ai, IDamageable _target)
    {
        _ai.Anim.PlayAnimation("Attack03");
        GameObject.Instantiate(poofEffect, (Vector2)_ai.DamageDealer.DamageDealerObject.transform.position - Vector2.up, poofEffect.transform.rotation);
        _ai.DamageDealer.DamageDealerObject.SetActive(false);

        var timer = Timer.CreateTimer(1f, () => false, false);
        timer.OnEnd += () =>
        {
            Jump(_ai, _target);
        };        
    }

    private void Jump(IAttacker _ai, IDamageable _target)
    {
        _ai.DamageDealer.DamageDealerObject.SetActive(true);
        var targetTransform = _target.DamageableObject.transform;
        _ai.DamageDealer.DamageDealerObject.transform.position = ((Vector2)targetTransform.position + Vector2.up) - (-(Vector2)targetTransform.forward * 2f);
        GameObject.Instantiate(poofEffect, (Vector2)_ai.DamageDealer.DamageDealerObject.transform.position - Vector2.up, poofEffect.transform.rotation);
    }
}
