using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEtherealEffect
{
    void OnShoot(FSMController _controller);
    void OnDrop(FSMController _controller);
    void OnPull(FSMController _controller);
    void OnGoto(FSMController _controller);

    void OnActivate(FSMController _controller);
    void OnDeactivate(FSMController _controller);

    void OnCollide(Collider _collider);
    void OnLinkCollideTick(Collider _collider);
}
