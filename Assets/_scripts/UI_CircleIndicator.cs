using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Elysium.Utils.Attributes;

[RequireComponent(typeof(LineRenderer))]
public class UI_CircleIndicator : MonoBehaviour, IRangeIndicator
{
    [SerializeField] private int segments = 50;
    [SerializeField] private float width = 0.05f;
    [SerializeField, ReadOnly] private LineRenderer lineRenderer = default;

    public float Radius { get; set; } = 5;

    public void SetActive(bool _active)
    {
        gameObject.SetActive(_active);
    }

    private void CreatePoints()
    {
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;

        float x = default;
        float y = default;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * Radius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * Radius;

            lineRenderer.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }

    private void OnValidate()
    {
        if (lineRenderer == null) { lineRenderer = gameObject.GetComponent<LineRenderer>(); }
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = false;
        CreatePoints();
    }
}
