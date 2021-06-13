using Elysium.Combat;
using Elysium.Utils.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireGroundArea : MonoBehaviour
{
    [SerializeField] private float damageMultiplier = 1f;

    private Collider collider = default;    
    private float tickInterval = 1f;
    private float tickTimer = 0f;

    private IDamageDealer caster = default;

    private List<Collider2D> CollidersInArea = default;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        CollidersInArea = new List<Collider2D>();
        gameObject.SetActive(false);
    }

    public void Enable(IDamageDealer _caster, float _damageMultiplier)
    {
        this.caster = _caster;
        this.damageMultiplier = _damageMultiplier;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        tickTimer -= Time.deltaTime;
        if (tickTimer <= 0)
        {
            tickTimer = tickInterval;
            Tick();
        }
    }

    public void Disable()
    {
        Timer.CreateTimer(1f, () => false, false).OnEnd += () => gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D _collision)
    {
        CollidersInArea.Add(_collision);
    }

    private void OnTriggerExit2D(Collider2D _collision)
    {
        CollidersInArea.Remove(_collision);
    }

    private void Tick()
    {
        if (caster == null) { return; }

        foreach (var col in CollidersInArea)
        {
            var damageable = col.GetComponentInChildren<IDamageable>();
            if (damageable != null)
            {
                if (!caster.DealsDamageToTeams.Contains(damageable.Team)) { continue; }
                damageable.TakeDamage(caster, Mathf.CeilToInt(caster.Damage.Value * damageMultiplier), "Fire");
            }
        }
    }
}
