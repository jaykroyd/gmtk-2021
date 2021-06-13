using Elysium.Combat;

public interface IAttack
{
    float Range { get; }

    void Attack(AI _ai, IDamageable _target);
}