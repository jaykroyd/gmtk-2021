using Elysium.Combat;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : IAttack
{
    private TimerInstance cooldownTimer = default;
    private float attackCooldown = 1f;

    public float Range { get; set; } = 1.5f;

    public MeleeAttack(float _range = 1.5f)
    {
        this.Range = _range;
        this.cooldownTimer = Timer.CreateEmptyTimer(() => false, true);
    }

    public void Attack(IAttacker _attacker, IDamageable _target)
    {
        if (!cooldownTimer.IsEnded) { return; }

        cooldownTimer.SetTime(attackCooldown);
        _attacker.Anim.PlayAnimation("Attack");
        _target.TakeDamage(_attacker.DamageDealer, _attacker.DamageDealer.Damage.Value);
    }
}
