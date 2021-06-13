using Elysium.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour, IDamageable
{
    [SerializeField] private float damageMultiplier = 3f;
    [SerializeField] HealthController healthController;

    public bool IsDead => healthController.IsDead;

    public DamageTeam Team => healthController.Team;

    public GameObject DamageableObject => healthController.DamageableObject;

    public event Action<IDamageDealer, int, string> OnTakeDamage;
    public event Action<IDamageDealer, int, string> OnHeal;
    public event Action OnHealthEmpty;
    public event Action OnDeathStatusChange;
    public event Action OnDeath;
    public event Action OnRespawn;

    private void Start()
    {
        healthController.OnTakeDamage += (a, b, c) => OnTakeDamage?.Invoke(a, b, c);
        healthController.OnHeal += (a, b, c) => OnHeal?.Invoke(a, b, c);
        healthController.OnHealthEmpty += () => OnHealthEmpty?.Invoke();
        healthController.OnDeathStatusChange += () => OnDeathStatusChange?.Invoke();
        healthController.OnDeath += () => OnDeath?.Invoke();
        healthController.OnRespawn += () => OnRespawn?.Invoke();
    }

    public bool Heal(IDamageDealer damageComponent, int damage, string source = "")
    {
        return healthController.Heal(damageComponent, damage, source);
    }

    public bool TakeDamage(IDamageDealer damageComponent, int damage, string source = "")
    {
        return healthController.TakeDamage(damageComponent, Mathf.CeilToInt(damage * damageMultiplier), source);
    }    
}
