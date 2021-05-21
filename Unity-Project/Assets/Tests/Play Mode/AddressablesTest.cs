// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.TestTools;

namespace BladeMR.Tests
{
    public class AddressablesTest : MonoBehaviour
    {
        private const string TestAssetPath = "Assets/Tests/Test Assets/Cube.prefab";

        /// <summary>
        /// Simple asset load test to ensure project addressables are setup correctly
        /// </summary>
        [UnityTest]
        public IEnumerator TestLoadFromPath()
        {
            var asyncHandle = Addressables.InstantiateAsync(TestAssetPath);
            yield return asyncHandle;
            Assert.IsNotNull(asyncHandle.Result);
            Assert.IsTrue(asyncHandle.Status == AsyncOperationStatus.Succeeded);
        }
    }
}