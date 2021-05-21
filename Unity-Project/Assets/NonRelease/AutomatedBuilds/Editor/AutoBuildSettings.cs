// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using UnityEngine;

/// <summary>
/// Scriptable Object defining which scenes will be in automated builds, and which one is the start scene.
/// </summary>
[CreateAssetMenu(fileName = "AutoBuildSettingsData", menuName = "Scriptable Objects/Non Release/Blade Autobuild Settings")]
public class AutoBuildSettings : ScriptableObject
{
    /// <summary>
    /// Array of paths to scenes to be included in the build. Values assigned in the inspector.
    /// </summary>
    public string[] ScenePathsForBuild;

    /// <summary>
    /// Index in the array of the scene to be used as the start scene.
    /// </summary>
    public int StartSceneZeroBasedIndex;
}