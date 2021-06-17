using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPart : MonoBehaviour, IPlayer
{
    [SerializeField] private Player player = default;

    public void CreateEarthEffect()
    {
        player.CreateEarthEffect();
    }

    public void CreateFireEffect()
    {
        player.CreateFireEffect();
    }

    public void CreateVineEffect()
    {
        player.CreateVineEffect();
    }

    public void CreateWaterEffect()
    {
        player.CreateWaterEffect();
    }

    public void CreateWindEffect()
    {
        player.CreateWindEffect();
    }

    public void ReceiveReward(RewardPackage rewardPackage)
    {
        player.ReceiveReward(rewardPackage);
    }
}
