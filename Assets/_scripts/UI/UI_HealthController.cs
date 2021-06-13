using Elysium.Combat;
using Elysium.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HealthController : MonoBehaviour
{
    [SerializeField] IntValueSO maxHealth;
    [SerializeField] IntValueSO currentHealth;

    private HealthController controller;

    private void Awake()
    {
        controller = GetComponent<HealthController>();

        if (maxHealth != null && currentHealth != null)
        {
            controller.OnFillValueChanged += UpdateHealth;
        }

        UpdateHealth();
    }

    private void UpdateHealth()
    {
        maxHealth.Value = (int)controller.Max;
        currentHealth.Value = (int)controller.Current;
    }
}
