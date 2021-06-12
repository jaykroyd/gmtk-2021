using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEffect : BaseEffect
{
    const float DISTANCE_BETWEEN_LINKS = 0.3f;
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
        Vector2 startPosition = (Vector2)ethereal.transform.position;
        Vector2 endPosition = (Vector2)controller.transform.position;

        Vector2 direction = (endPosition - startPosition).normalized;

        int n = Mathf.CeilToInt(Vector2.Distance(startPosition, endPosition) / DISTANCE_BETWEEN_LINKS);
        float dist = Vector2.Distance(startPosition, endPosition) / n;

        objs = new GameObject[n+1];
        objs[0] = MonoBehaviour.Instantiate(vineTopPrefab, startPosition, Quaternion.LookRotation(direction, Vector3.back), ethereal.transform);
        for (int i = 0; i < n; i++)
        {
            objs[i+1] = MonoBehaviour.Instantiate(vineLinkPrefab, startPosition + direction * (i + 0.5f), Quaternion.LookRotation(direction, Vector3.back), ethereal.transform);
            objs[i+1].transform.localScale = DISTANCE_BETWEEN_LINKS * new Vector3(1,1,1);
            objs[i+1].GetComponent<HingeJoint2D>().connectedBody = objs[i].GetComponent<Rigidbody2D>();
        }
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
