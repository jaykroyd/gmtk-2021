using Elysium.Utils.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Elysium.Utils;

[RequireComponent(typeof(LineRenderer))]
public class SpectralLink : MonoBehaviour
{
    [SerializeField] private float startWidth = 0.2f;
    [SerializeField] private float endWidth = 0.2f;
    [SerializeField] private float zOffset = 1f;
    [SerializeField, ReadOnly] private FSMController controller = default;
    [SerializeField, ReadOnly] private LineRenderer lineRenderer = default;

    public float Radius { get; set; } = 5;

    public void SetColor(Color _color)
    {
        lineRenderer.material.SetColor("_Color", _color);
    }

    public void SetActive(bool _active)
    {
        gameObject.SetActive(_active);
    }

    private void StartPosition()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position.SetZ(zOffset));
    }

    private void EndPosition()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(1, controller.transform.position.SetZ(zOffset));
    }

    private void LateUpdate()
    {
        StartPosition();
        EndPosition();
    }

    private void OnValidate()
    {
        if (controller == null) { controller = FindObjectOfType<FSMController>(); }
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
