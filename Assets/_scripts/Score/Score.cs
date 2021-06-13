using Elysium.Utils.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private float activationTime = 0.7f;

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
        if (_collision.collider.TryGetComponent(out Player _player))
        {
            _player.ReceiveReward(new RewardPackage
            {
                Score = 1,
            });

            Destroy(gameObject);
        }
    }    
}
