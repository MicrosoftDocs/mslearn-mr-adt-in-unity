// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

public class RotateRPM : MonoBehaviour
{
    public float rpm = 10f;

    private void Update()
    {
        transform.Rotate (Vector3.forward, rpm * 6f * Time.deltaTime);
    }
}
