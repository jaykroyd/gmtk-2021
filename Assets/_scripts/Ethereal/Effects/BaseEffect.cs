using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect : IEtherealEffect
{
    protected Ethereal ethereal = default;
    protected FSMController controller = default;

    protected Color mainColor = default;
    protected Color linkColor = default;

    public BaseEffect(FSMController _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor)
    {
        controller = _controller;
        ethereal = _ethereal;
        this.mainColor = _mainColor;
        this.linkColor = _linkColor;
    }

    public abstract void OnActivate();

    public abstract void OnCollide(Collider _collider);

    public abstract void OnLinkCollideTick(Collider _collider);

    public abstract void OnDeactivate();

    public abstract void OnDrop();

    public abstract void OnGoto();

    public abstract void OnPull();

    public abstract void OnShoot();
}
