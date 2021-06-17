public interface IPlayer
{
    void ReceiveReward(RewardPackage rewardPackage);
    void CreateFireEffect();
    void CreateWaterEffect();
    void CreateWindEffect();
    void CreateVineEffect();
    void CreateEarthEffect();
}