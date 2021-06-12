using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect : IEtherealEffect
{
    protected Ethereal ethereal = default;
    protected Player controller = default;

    protected Color mainColor = default;
    protected Color linkColor = default;

    public BaseEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor)
    {
        controller = _controller;
        ethereal = _ethereal;
        this.mainColor = _mainColor;
        this.linkColor = _linkColor;
    }

    public abstract void OnActivate();

    public abstract void OnCollide(Collider2D _collider);

    public abstract void OnLinkCollideTick(Collider2D _collider);

    public abstract void OnDeactivate();

    public abstract void OnDrop();

    public abstract void OnGotoStart();

    public abstract void OnGotoEnd();

    public abstract void OnPullStart();

    public abstract void OnPullEnd();

    public abstract void OnShootStart();

    public abstract void OnShootEnd();
}
