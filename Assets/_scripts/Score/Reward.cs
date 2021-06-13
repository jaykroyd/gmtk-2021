using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    [SerializeField] private Score scorePrefab = default;

    public void CreateRewards(RewardPackage _reward, float _spawnRadius = 2f)
    {
        for (int i = 0; i < _reward.Score; i++)
        {
            Vector2 pos = Vector2.zero;
            int j = 0;
            do
            {
                pos = Random.insideUnitCircle * _spawnRadius;
                j++;
            }
            while (pos.y < 0 || j > 100);
            
            Instantiate(scorePrefab, (Vector2)transform.position + pos, scorePrefab.transform.rotation);
        }

        Destroy(gameObject);
    }

    public static Reward Create(Reward _prefab, Vector2 _position, RewardPackage _rewards, float _spawnRadius = 2f)
    {
        Reward r = Instantiate(_prefab, _position, _prefab.transform.rotation);
        r.CreateRewards(_rewards, _spawnRadius);
        return r;
    }
}
