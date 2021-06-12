using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseEffect : IEtherealEffect
{
    protected Ethereal ethereal = default;
    protected Player controller = default;

    protected Color mainColor = default;
    protected Color linkColor = default;

    protected int modelIndex = default;

    public event UnityAction OnShootEnd;

    public BaseEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex)
    {
        controller = _controller;
        ethereal = _ethereal;
        this.mainColor = _mainColor;
        this.linkColor = _linkColor;
        this.modelIndex = _modelIndex;
    }

    public virtual void OnActivate()
    {
        ethereal.SetModel(modelIndex);
        ethereal.Link.SetColor(linkColor);
        controller.SetParticles(modelIndex);
    }
    public virtual void OnDeactivate()
    {
        controller.SetParticles(-1);
    }

    public abstract void OnCollide(Collider2D _collider);

    public abstract void OnLinkCollideTick(Collider2D _collider);

    public abstract void DeployStart();

    public abstract void DeployFinish();

    public abstract void RetrieveStart();

    public abstract void RetrieveFinish();

    protected virtual void Shoot()
    {
        ethereal.transform.position = controller.transform.position;
        var worldPos = Utils.GetCurrentMousePosition();
        worldPos = new Vector3(worldPos.x, worldPos.y, 0);
        Vector2 direction = worldPos - (Vector2)ethereal.transform.position;
        ethereal.Destination = (Vector2)ethereal.transform.position + (direction.normalized * ethereal.MaxDistance);

        void OnArriveAtDestination()
        {
            ethereal.IsDeployed = true;
            ethereal.OnDestinationArrival -= DeployFinish;
            ethereal.OnDestinationArrival -= OnArriveAtDestination;
        }

        ethereal.OnDestinationArrival += DeployFinish;
        ethereal.OnDestinationArrival += OnArriveAtDestination;
    }

    protected virtual void Drop()
    {
        ethereal.transform.position = controller.transform.position;
        ethereal.IsDeployed = true;
        DeployFinish();
    }

    protected virtual void Pull()
    {
        ethereal.Target = controller.transform;

        void DeactivateOnArrival()
        {
            ethereal.OnPlayerArrival -= RetrieveFinish;
            ethereal.OnPlayerArrival -= DeactivateOnArrival;
            ethereal.Deactivate();
        }

        ethereal.OnPlayerArrival += RetrieveFinish;
        ethereal.OnPlayerArrival += DeactivateOnArrival;
    }

    protected virtual void GoTo()
    {
        controller.Destination = (Vector2)ethereal.transform.position;
        controller.Movement.MoveSpeed = 30f;

        void DeactivateOnArrival()
        {
            ethereal.OnPlayerArrival -= RetrieveFinish;
            ethereal.OnPlayerArrival -= DeactivateOnArrival;            
            ethereal.Deactivate();
        }

        ethereal.OnPlayerArrival += RetrieveFinish;
        ethereal.OnPlayerArrival += DeactivateOnArrival;        
    }
}
