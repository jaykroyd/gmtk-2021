using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VineEffect : MonoBehaviour, IEtherealEffect
{
    FSMController controller;
    Ethereal ethereal;
    Color color;

    const float DISTANCE_BETWEEN_LINKS = 0.3f;
    GameObject vineLinkPrefab = null;
    GameObject vineTopPrefab = null;
    GameObject[] objs = null;

    public VineEffect(FSMController _controller, Ethereal _ethereal, Color _color)
    {
        controller = _controller;
        ethereal = _ethereal;
        this.color = _color;
    }

    public void OnCollide(Collider _collider)
    {
        Vector2 startPosition = (Vector2)ethereal.transform.position;
        Vector2 endPosition = (Vector2)controller.transform.position;

        Vector2 direction = (endPosition - startPosition).normalized;

        int n = Mathf.CeilToInt(Vector2.Distance(startPosition, endPosition) / DISTANCE_BETWEEN_LINKS);
        float dist = Vector2.Distance(startPosition, endPosition) / n;

        objs = new GameObject[n+1];
        objs[0] = Instantiate(vineTopPrefab, startPosition, Quaternion.LookRotation(direction, Vector3.back), this.transform);
        for (int i = 0; i < n; i++)
        {
            objs[i+1] = Instantiate(vineLinkPrefab, startPosition + direction * (i + 0.5f), Quaternion.LookRotation(direction, Vector3.back), this.transform);
            objs[i+1].transform.localScale = DISTANCE_BETWEEN_LINKS * new Vector3(1,1,1);
            objs[i+1].GetComponent<HingeJoint2D>().connectedBody = objs[i].GetComponent<Rigidbody2D>();
        }
    }

    public void OnLinkCollideTick(Collider _collider)
    {
    }

    public void OnActivate()
    {
        color.a = 0.3f;
        ethereal.Renderer.color = color;
    }

    public void OnDeactivate()
    {
        for (int i = 0; i < objs.Length; i++)
        {
            Destroy(objs[i]);
        }
        objs = null;
    }

    public void OnReachDestination()
    {
        ethereal.Pull(controller);
    }

    public void OnDrop()
    {
    }

    public void OnGoto()
    {
    }

    public void OnPull()
    {
    }

    public void OnShoot()
    {
    }

}
