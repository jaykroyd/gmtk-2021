using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEtherealEffect
{
    void DeployStart();
    void DeployFinish();
    void RetrieveStart();
    void RetrieveFinish();

    void OnActivate();
    void OnDeactivate();

    void OnCollide(Collider2D _collider);
    void OnLinkCollideTick(Collider2D _collider);
}
