using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    [SerializeField] private Score scorePrefab = default;

    [SerializeField] private Power fireEffect = default;
    [SerializeField] private Power earthEffect = default;
    [SerializeField] private Power vineEffect = default;
    [SerializeField] private Power waterEffect = default;
    [SerializeField] private Power windEffect = default;

    public void CreateRewards(RewardPackage _reward, float _spawnRadius = 2f)
    {
        for (int i = 0; i < _reward.Score; i++)
        {
            InstantiatePrefab(_spawnRadius, scorePrefab);
        }

        if (_reward.Spirit == Power.SpiritType.Fire) { InstantiatePrefab(_spawnRadius, fireEffect); }
        if (_reward.Spirit == Power.SpiritType.Water) { InstantiatePrefab(_spawnRadius, waterEffect); }
        if (_reward.Spirit == Power.SpiritType.Wind) { InstantiatePrefab(_spawnRadius, windEffect); }
        if (_reward.Spirit == Power.SpiritType.Vine) { InstantiatePrefab(_spawnRadius, vineEffect); }
        if (_reward.Spirit == Power.SpiritType.Earth) { InstantiatePrefab(_spawnRadius, earthEffect); }

        Destroy(gameObject);
    }

    private void InstantiatePrefab(float _spawnRadius, MonoBehaviour _prefab)
    {
        Vector2 pos = Vector2.zero;
        int j = 0;
        do
        {
            pos = Random.insideUnitCircle * _spawnRadius;
            j++;
        }
        while (pos.y < 0 || j > 100);

        Instantiate(_prefab, (Vector2)transform.position + pos, scorePrefab.transform.rotation);
    }

    public static Reward Create(Reward _prefab, Vector2 _position, RewardPackage _rewards, float _spawnRadius = 2f)
    {
        Reward r = Instantiate(_prefab, _position, _prefab.transform.rotation);
        r.CreateRewards(_rewards, _spawnRadius);
        return r;
    }
}
