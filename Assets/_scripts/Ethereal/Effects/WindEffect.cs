using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindEffect : BaseEffect
{
    private float gravityScale = 0;
    private float prevGravityScale = 0;

    private float pushbackRange = 4f;
    private float pushbackForce = 1f;

    public WindEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _gravityScale, float _pushbackRange, float _pushbackForce) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm)
    {
        this.gravityScale = _gravityScale;
        this.pushbackRange = _pushbackRange;
        this.pushbackForce = _pushbackForce;
    }

    public override void OnActivate()
    {
        base.OnActivate();

        prevGravityScale = controller.Rigidbody.gravityScale;
        controller.Rigidbody.gravityScale = gravityScale;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        controller.Rigidbody.gravityScale = prevGravityScale;
    }

    public override void OnCollide(Collider2D _collider)
    {
        
    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {

    }

    public override void DeployStart()
    {
        Drop();
    }

    public override void DeployFinish()
    {
        
    }

    public override void RetrieveStart()
    {
        Pull();
    }

    public override void RetrieveFinish()
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
}
