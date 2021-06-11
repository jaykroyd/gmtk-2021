using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class UI_LineIndicator : MonoBehaviour, IRangeIndicator
{
    [SerializeField] private float startWidth = 0.2f;
    [SerializeField] private float endWidth = 0.2f;
    [SerializeField, ReadOnly] private LineRenderer lineRenderer = default;

    public float Radius { get; set; } = 5;

    public void SetActive(bool _active)
    {
        gameObject.SetActive(_active);
    }

    private void StartPosition()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);        
    }

    private void EndPosition()
    {
        lineRenderer.positionCount = 2;

        //Vector2 worldMousePos = GetWorldPositionFromMousePosition();
        //Vector3 currentDirection = worldMousePos - (Vector2)transform.position;
        //currentDirection = currentDirection.normalized;

        //currentDirection = currentDirection * Vector2.Distance(transform.position, worldMousePos);
        //Vector3 endPos = Vector3.ClampMagnitude(currentDirection, Radius);
        //endPos.z = 0;

        Vector2 currentPos = GetWorldPositionFromMousePosition();
        Vector2 maxPos = (Vector2)transform.position + (currentPos.normalized * Radius);

        Vector2 endPos = Vector2.Min(currentPos, maxPos);
        Vector3 finalEndPos = new Vector3(endPos.x, endPos.y, 0);

        lineRenderer.SetPosition(1, finalEndPos);
    }

    private void Update()
    {
        StartPosition();
        EndPosition();
    }

    private void OnValidate()
    {
        if (lineRenderer == null) { lineRenderer = GetComponent<LineRenderer>(); }
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    private Vector2 GetWorldPositionFromMousePosition()
    {
        var mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        var worldPosition = Camera.main.ViewportToWorldPoint(new Vector3(mousePos.x / Screen.width, mousePos.y / Screen.height, 10.0f));
        return worldPosition;
    }
}
