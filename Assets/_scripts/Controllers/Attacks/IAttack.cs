using Elysium.Combat;

public interface IAttack
{
    float Range { get; }

    void Attack(IAttacker _ai, IDamageable _target);
}