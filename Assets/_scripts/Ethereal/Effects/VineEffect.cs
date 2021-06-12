using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEffect : BaseEffect
{
    bool isAttached = false;

    [SerializeField] float MaxDistance = 10f;
    [SerializeField] float ForceMultiplier = 100000f;
    float VineLength = 0;

    Rigidbody2D rb;

    public VineEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, float _timeInForm, GameObject _vineLinkPrefab, GameObject _vineTopPrefab) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex, _timeInForm)
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

        if (_collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
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
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        timer.OnTick -= Update;
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
        
    }
}
