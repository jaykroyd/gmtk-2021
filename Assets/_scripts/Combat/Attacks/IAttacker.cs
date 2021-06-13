using Elysium.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttacker
{
    IDamageDealer DamageDealer { get; }
    IModelController Anim { get; }
}
