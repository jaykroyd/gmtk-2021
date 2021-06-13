using Elysium.UI.ProgressBar;
using Elysium.Utils.Timers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseEffect : IEtherealEffect, IFillable
{
    protected Ethereal ethereal = default;
    protected Player controller = default;

    protected Color mainColor = default;
    protected Color linkColor = default;

    protected TimerInstance timer = default;
    protected TimerInstance cooldownTimer = default;

    protected int modelIndex = default;

    protected virtual float maxDurationInSpiritForm { get; set; } = 5f;

    public float Current { get; set; }
    public float Max { get; set; }

    public event UnityAction OnFillValueChanged;

    public BaseEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _cooldown)
    {
        controller = _controller;
        ethereal = _ethereal;
        this.mainColor = _mainColor;
        this.linkColor = _linkColor;
        this.modelIndex = _modelIndex;
        this.maxDurationInSpiritForm = _timeInForm;
        this.Max = _cooldown;
        Current = 0;

        cooldownTimer = Timer.CreateEmptyTimer(() => !ethereal, true);
        OnFillValueChanged?.Invoke();

        cooldownTimer.OnTick += () =>
        {
            Current = cooldownTimer.Time;
            OnFillValueChanged?.Invoke();
        };
        cooldownTimer.OnEnd += () =>
        {
            Current = 0;
            OnFillValueChanged?.Invoke();
        };
    }

    public bool IsAvailable => cooldownTimer.IsEnded;

    public virtual void OnActivate()
    {
        if (!IsAvailable)
        {
            Debug.LogError("started unavailable skill!");
        }

        cooldownTimer.SetTime(Max);
        ethereal.SetModel(modelIndex);
        ethereal.Link.SetColor(linkColor);
        controller.SetParticles(modelIndex);
        timer = Timer.CreateTimer(maxDurationInSpiritForm, () => !ethereal, false);
        timer.OnEnd += RetrieveStart;
    }

    public virtual void OnDeactivate()
    {
        controller.SetParticles(-1);
        timer.EndSilent();
        timer.Dispose();
    }

    public abstract void OnCollide(Collider2D _collider);

    public abstract void OnLinkCollideTick(Collider2D _collider);

    public abstract void DeployStart();

    public abstract void DeployFinish();

    public abstract void RetrieveStart();

    public abstract void RetrieveFinish();

    public void ForceRetrieve()
    {
        controller.Movement.MoveSpeed = 10f;
        Pull();
    }

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

    public void TriggerOnFillValueChanged()
    {
        OnFillValueChanged?.Invoke();
    }
}
