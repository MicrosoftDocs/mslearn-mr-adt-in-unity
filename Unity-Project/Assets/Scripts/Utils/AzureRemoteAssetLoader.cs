// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AzureRemoteAssetLoader : MonoBehaviour
{
    public string addressableKey;
    public GameObject parent;
    public GameObject placeholder;

    private void Start()
    {
        Addressables.InstantiateAsync(addressableKey, parent.transform).Completed += OnInstantiateDone;
    }

    public void OnInstantiateDone(UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<GameObject> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            placeholder.SetActive(false);
        }
        else
            Debug.LogError("Addressable instantiate failed");
    }
}