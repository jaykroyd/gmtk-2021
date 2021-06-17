using Elysium.Combat;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGroundAttack : IAttack
{
    private FireGroundArea[] fireGround = default;
    private float fireGroundAttackDuration = 5f;
    private float fireGroundAttackDamageMultiplier = 5f;
    private TimerInstance fireGroundAttackTimer = default;

    public FireGroundAttack(FireGroundArea[] _fireGround, float _fireGroundAttackDuration, float _fireGroundAttackDamageMultiplier)
    {
        this.fireGround = _fireGround;
        this.fireGroundAttackDuration = _fireGroundAttackDuration;
        this.fireGroundAttackDamageMultiplier = _fireGroundAttackDamageMultiplier;
        fireGroundAttackTimer = Timer.CreateEmptyTimer(() => false, true);
        fireGroundAttackTimer.OnEnd += EndAttack;
    }

    public float Range => Mathf.Infinity;

    public void Attack(IAttacker _ai, IDamageable _target)
    {
        _ai.Anim.PlayAnimation("Attack03");        
        fireGroundAttackTimer.SetTime(fireGroundAttackDuration);

        foreach (var g in fireGround)
        {
            g.Enable(_ai as IDamageDealer, fireGroundAttackDamageMultiplier);
        }
    }
    
    private void EndAttack()
    {
        foreach (var g in fireGround)
        {
            g.Disable();
        }
    }
}
