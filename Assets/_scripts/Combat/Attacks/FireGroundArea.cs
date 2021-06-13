using Elysium.Combat;
using Elysium.Utils.Timers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGroundArea : MonoBehaviour
{
    private Collider collider = default;
    private float tickInterval = 1f;
    private float tickTimer = 0f;

    private List<Collider2D> CollidersInArea = default;

    private void Awake()
    {
        collider = GetComponent<Collider>();
        CollidersInArea = new List<Collider2D>();
    }

    public void Enable()
    {
        gameObject.SetActive(true);
    }

    private void Update()
    {
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
        foreach (var col in CollidersInArea)
        {
            var damageable = col.GetComponentInChildren<IDamageable>();
        }
    }
}
