using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEffect : BaseEffect
{
    const float DISTANCE_BETWEEN_LINKS = 0.1f;
    GameObject vineLinkPrefab = null;
    GameObject vineTopPrefab = null;
    GameObject[] objs = null;

    public VineEffect(Player _controller, Ethereal _ethereal, Color _mainColor, Color _linkColor, int _modelIndex, GameObject _vineLinkPrefab, GameObject _vineTopPrefab) : base(_controller, _ethereal, _mainColor, _linkColor, _modelIndex)
    {
        vineLinkPrefab = _vineLinkPrefab;
        vineTopPrefab = _vineTopPrefab;
}

    public override void OnCollide(Collider2D _collider)
    {
        if(objs != null)
            return;

        if (_collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return;

        Vector2 startPosition = (Vector2)ethereal.transform.position;
        Vector2 endPosition = (Vector2)controller.transform.position;

        Vector2 direction = (endPosition - startPosition).normalized;
        float distance = Vector2.Distance(startPosition, endPosition);

        int n = Mathf.CeilToInt(distance / DISTANCE_BETWEEN_LINKS);
        float spacing = distance / n;

        objs = new GameObject[n+1];
        objs[0] = MonoBehaviour.Instantiate(vineTopPrefab, startPosition, Quaternion.LookRotation(Vector3.forward, -direction), ethereal.transform);
        HingeJoint2D hinge;
        for (int i = 0; i < n; i++)
        {
            objs[i+1] = MonoBehaviour.Instantiate(vineLinkPrefab, startPosition + direction * spacing * (i + 0.5f), Quaternion.LookRotation(Vector3.forward, -direction), ethereal.transform);
            objs[i+1].transform.localScale = DISTANCE_BETWEEN_LINKS * new Vector3(1,1,1);
            hinge = objs[i+1].GetComponent<HingeJoint2D>();
            hinge.connectedBody = objs[i].GetComponent<Rigidbody2D>();
            
        }
        //TODO: Add bottom hinge-joing for swinging player
        
        hinge = objs[n].AddComponent<HingeJoint2D>();
        hinge.connectedBody = controller.GetComponent<Rigidbody2D>();
        hinge.anchor = new Vector2(0,-0.5f);

        DistanceJoint2D restriction = objs[n].AddComponent<DistanceJoint2D>();
        restriction.connectedBody = controller.GetComponent<Rigidbody2D>();
        restriction.distance = distance * 1.1f;
        restriction.maxDistanceOnly = true;
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
        if (objs == null) { return; }
        for (int i = 0; i < objs.Length; i++)
        {
            MonoBehaviour.Destroy(objs[i]);
        }
        objs = null;
    }

    public override void DeployStart()
    {
        Shoot();
    }

    public override void DeployFinish()
    {
        
    }

    public override void RetrieveStart()
    {
        GoTo();
    }

    public override void RetrieveFinish()
    {
        
    }
}
