using Elysium.Combat;
using Elysium.Utils;
using Elysium.Utils.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spikes : MonoBehaviour, IDamageDealer
{
    [SerializeField] int damage = 1000;
    [SerializeField] DamageTeam[] dealsDamageToTeams;

    public RefValue<int> Damage { get; set; } = new RefValue<int>(() => 100);
    public DamageTeam[] DealsDamageToTeams => dealsDamageToTeams;
    public GameObject DamageDealerObject => gameObject;

    private List<IDamageable> TargetsInCollider = default;

    private TimerInstance tickTimer = default;

    private void Awake()
    {
        TargetsInCollider = new List<IDamageable>();
        Damage = new RefValue<int>(() => damage);        
        var tickInterval = 1f;

        tickTimer = Timer.CreateTimer(tickInterval, () => !this, true);
        tickTimer.OnEnd += () => tickTimer.SetTime(tickInterval);
        tickTimer.OnEnd += Tick;
    }

    public void CriticalHit()
    {
        
    }

    private void Tick()
    {
        foreach (var dmg in TargetsInCollider)
        {
            dmg.TakeDamage(this, Damage.Value, "Spike");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Entered Spikes");
        var damageable = collision.collider.transform.root.GetComponentInChildren<IDamageable>();
        if (damageable != null && dealsDamageToTeams.Contains(damageable.Team))
        {
            TargetsInCollider.Add(damageable);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("Left Spikes");
        var damageable = collision.collider.transform.root.GetComponentInChildren<IDamageable>();
        if (damageable != null && dealsDamageToTeams.Contains(damageable.Team))
        {
            TargetsInCollider.Remove(damageable);
        }
    }
}
