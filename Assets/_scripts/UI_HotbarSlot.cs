using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HotbarSlot : MonoBehaviour
{
    [SerializeField] private Image highlight = default;
    [SerializeField] private Color highlightColor = Color.yellow;

    private Color defaultColor = default;

    private void Awake()
    {
        defaultColor = highlight.color;
    }

    public void Highlight(bool _active)
    {
        highlight.color = _active ? highlightColor : defaultColor;        
    }
}
