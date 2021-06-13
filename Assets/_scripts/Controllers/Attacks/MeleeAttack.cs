using Elysium.Combat;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : IAttack
{
    private TimerInstance cooldownTimer = default;
    private float attackCooldown = 1f;

    public float Range => 1.5f;

    public MeleeAttack()
    {
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
