// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Enables/Disables the linked object when the map pin come in/out of view.
/// </summary>
public class MapPinLinkedObject : MonoBehaviour
{
    public GameObject linkedObject;
    
    private void OnEnable()
    {
        if (linkedObject != null)
        {
            linkedObject.SetActive(true);
        }
    }

    private void OnDisable()
    {
        if (linkedObject != null)
        {
            linkedObject.SetActive(false);
        }
    }
}
