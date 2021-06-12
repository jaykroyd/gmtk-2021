using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : BaseEffect
{
    private float gravityScale = 0;
    private float prevGravityScale = 0;

    private float pushbackRange = 4f;
    private float pushbackForce = 1f;

    public WindEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, float _gravityScale, float _pushbackRange, float _pushbackForce) : base(_controller, _ethereal, _mainColor, _linkColor)
    {
        this.gravityScale = _gravityScale;
        this.pushbackRange = _pushbackRange;
        this.pushbackForce = _pushbackForce;
    }

    public override void OnActivate()
    {
        mainColor.a = 0.3f;
        ethereal.Renderer.color = mainColor;
        ethereal.Link.SetColor(linkColor);

        prevGravityScale = controller.Rigidbody.gravityScale;
        controller.Rigidbody.gravityScale = gravityScale;
    }

    public override void OnCollide(Collider2D _collider)
    {
        
    }

    public override void OnDeactivate()
    {
        controller.Rigidbody.gravityScale = prevGravityScale;
    }

    public override void OnDrop()
    {
        
    }

    public override void OnGotoEnd()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(ethereal.transform.position, pushbackRange);
        foreach (var col in colliders)
        {
            if (col.TryGetComponent(out IPushable _pushable))
            {
                Vector2 direction = (Vector2)col.transform.position - (Vector2)ethereal.transform.position;
                _pushable.Push(pushbackForce, direction.normalized);
            }
        }
    }

    public override void OnGotoStart()
    {
        
    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {
        
    }

    public override void OnPullEnd()
    {
        
    }

    public override void OnPullStart()
    {
        
    }

    public override void OnShootEnd()
    {
        
    }

    public override void OnShootStart()
    {
        
    }
}
