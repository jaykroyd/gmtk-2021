using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEtherealEffect
{
    void OnShootStart();
    void OnShootEnd();
    void OnDrop();
    void OnPullStart();
    void OnPullEnd();
    void OnGotoStart();
    void OnGotoEnd();

    void OnActivate();
    void OnDeactivate();

    void OnCollide(Collider2D _collider);
    void OnLinkCollideTick(Collider2D _collider);
}
