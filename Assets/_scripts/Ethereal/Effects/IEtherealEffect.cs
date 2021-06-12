using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEtherealEffect
{
    void OnShoot();
    void OnDrop();
    void OnPull();
    void OnGoto();

    void OnActivate();
    void OnDeactivate();

    void OnCollide(Collider _collider);
    void OnLinkCollideTick(Collider _collider);
}
