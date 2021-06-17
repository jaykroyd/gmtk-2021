using Elysium.Utils.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    [SerializeField] private float activationTime = 0.7f;
    [SerializeField] private SpiritType spirit = default;

    public enum SpiritType
    {
        None,
        Fire,
        Water,
        Wind,
        Vine,
        Earth,
    }

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("NoCollision");
        StartCoroutine(ActivationRoutine());
    }

    IEnumerator ActivationRoutine()
    {
        yield return new WaitForSeconds(activationTime);
        gameObject.layer = LayerMask.NameToLayer("Reward");
        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        TryCollect(_collision);
    }
    private void OnCollisionStay2D(Collision2D _collision)
    {
        TryCollect(_collision);
    }

    private void TryCollect(Collision2D _collision)
    {
        if (_collision.collider.TryGetComponent(out IPlayer _player))
        {
            if (spirit == SpiritType.Fire) { _player.CreateFireEffect(); }
            if (spirit == SpiritType.Water) { _player.CreateWaterEffect(); }
            if (spirit == SpiritType.Wind) { _player.CreateWindEffect(); }
            if (spirit == SpiritType.Vine) { _player.CreateVineEffect(); }
            if (spirit == SpiritType.Earth) { _player.CreateEarthEffect(); }

            Destroy(gameObject);
        }
    }
}
