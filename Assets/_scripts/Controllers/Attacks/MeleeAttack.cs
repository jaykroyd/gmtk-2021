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

    public void Attack(AI _ai, IDamageable _target)
    {
        if (!cooldownTimer.IsEnded) { return; }

        cooldownTimer.SetTime(attackCooldown);
        _ai.Anim.PlayAnimation("Attack");
        _target.TakeDamage(_ai, _ai.Damage.Value);
    }
}
