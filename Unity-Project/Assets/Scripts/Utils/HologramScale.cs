// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using UnityEngine;

/// <summary>
/// Scales a real world size object down to holographic size
/// </summary>
[ExecuteInEditMode]
public class HologramScale : MonoBehaviour
{
    [Tooltip("The real world diameter of the object in  meters")]
    public float realWorldScale;

    [Tooltip("The desired size of the object in meters")]
    public float hologramScale = 1f;
    
    //OnValidate is called when the script is loaded or a value is changed in the Inspector
    private void OnValidate()
    {
        RecalculateScale();
    }

    private void Awake()
    {
        RecalculateScale();
    }

    private void RecalculateScale()
    {
        var scale = hologramScale / realWorldScale;
        if (!float.IsNaN(scale) && !float.IsInfinity(scale) && scale > 0)
        {
            transform.localScale = Vector3.one * scale;
        }
    }
}
