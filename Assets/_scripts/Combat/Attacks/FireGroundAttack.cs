using Elysium.Combat;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGroundAttack : IAttack
{
    private FireGroundArea fireGround = default;
    private float fireGroundAttackDuration = 5f;
    private TimerInstance fireGroundAttackTimer = default;

    public FireGroundAttack(FireGroundArea _fireGround, float _fireGroundAttackDuration)
    {
        this.fireGround = _fireGround;
        this.fireGroundAttackDuration = _fireGroundAttackDuration;
        fireGroundAttackTimer = Timer.CreateEmptyTimer(() => false, true);
        fireGroundAttackTimer.OnEnd += EndAttack;
    }

    public float Range => Mathf.Infinity;

    public void Attack(IAttacker _ai, IDamageable _target)
    {
        fireGround.Enable();
        fireGroundAttackTimer.SetTime(fireGroundAttackDuration);
    }
    
    private void EndAttack()
    {
        fireGround.Disable();
    }
}
