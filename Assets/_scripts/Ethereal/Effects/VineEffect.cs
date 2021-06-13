using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEffect : BaseEffect
{
    protected bool isAttached = false;

    [SerializeField] float MaxDistance = 10f;
    [SerializeField] float ForceMultiplier = 100000f;
    float VineLength = 0;

    Rigidbody2D rb;

    public VineEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, float _cooldown) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm, _cooldown)
    {
        rb = _controller.GetComponent<Rigidbody2D>();
    }

    float DistanceToEthereal => Vector2.Distance((Vector2)controller.transform.position, (Vector2)ethereal.transform.position);
    Vector3 GetVineDirection => ((Vector2)ethereal.transform.position - (Vector2)controller.transform.position).normalized;

    void Update()
    {

        if (DistanceToEthereal > VineLength)
        {
            float excess = DistanceToEthereal - VineLength;
            float force = ForceMultiplier * Mathf.Exp(excess);
            Debug.Log("Moving too far from vine!"+force.ToString());
            rb.AddForce(GetVineDirection * force * Time.deltaTime);
        }

    }

    public override void OnCollide(Collider2D _collider)
    {
        if(isAttached)
            return;
            
        bool isInLayer = ethereal.Movement.WhatIsGround.value == (ethereal.Movement.WhatIsGround.value | (1 << _collider.gameObject.layer));
        if (!isInLayer)
            return;

        Debug.Log("Attaching");

        isAttached = true;
        VineLength = DistanceToEthereal;
        
        timer.OnTick += Update;
    }

    public override void OnLinkCollideTick(Collider2D _collider)
    {

    }

    public override void OnActivate()
    {
        base.OnActivate();
        controller.Movement.AirControl = false;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        timer.OnTick -= Update;
        controller.Movement.AirControl = true;
    }

    public override void DeployStart()
    {
        isAttached = false;
        Shoot();
    }

    public override void DeployFinish()
    {
        if(!isAttached)
            Pull();
    }

    public override void RetrieveStart()
    {
        if(!isAttached)
            return;

        GoTo();
    }

    public override void RetrieveFinish()
    {
        controller.airJump = true;
        Elysium.Utils.Timers.Timer.CreateTimer(2f, ()=>false, false).OnEnd += 
        () =>
        {
            if(controller) 
                controller.airJump = false;
        };
    }
}
