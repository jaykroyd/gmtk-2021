using Elysium.UI.ProgressBar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_HotbarSlot : MonoBehaviour
{
    [SerializeField] private Image highlight = default;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Color defaultColor = default;

    public void SetupCooldownBar(IFillable _fillable)
    {
        defaultColor = highlight.color;
        gameObject.SetActive(true);
        GetComponent<ProgressBarFillableValue>().SetRuntimeData(_fillable);
    }

    public void Highlight(bool _active)
    {
        highlight.color = _active ? highlightColor : defaultColor;        
    }
}
