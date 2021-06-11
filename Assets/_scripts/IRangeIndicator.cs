using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRangeIndicator
{
    float Radius { get; set; }
    void SetActive(bool _active);
}
