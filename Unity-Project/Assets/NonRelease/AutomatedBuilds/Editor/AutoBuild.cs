using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

/// <summary>
/// Class used by the automated build process, to execute the build through the command line.
/// </summary>
public class AutoBuild
{
    /// <summary>
    /// Build method to create a build for the HoloLens 2.
    /// </summary>
    public static void BuildHoloLens2()
    {
        string[] autobuildSettingsAssetGuids = AssetDatabase.FindAssets($"t:{nameof(AutoBuildSettings)}");
        string path = AssetDatabase.GUIDToAssetPath(autobuildSettingsAssetGuids[0]);
        AutoBuildSettings settings = AssetDatabase.LoadAssetAtPath(path,typeof(AutoBuildSettings)) as AutoBuildSettings;

        List<EditorBuildSettingsScene> scenesForBuild = new List<EditorBuildSettingsScene>();

        int i = 0;
        foreach (var scenePath in settings.ScenePathsForBuild)
        {
            if (i == settings.StartSceneZeroBasedIndex)
                scenesForBuild = scenesForBuild.Prepend(new EditorBuildSettingsScene(scenePath, true)).ToList(); //add the start scene at the beginning
            else
                scenesForBuild = scenesForBuild.Append(new EditorBuildSettingsScene(scenePath, true)).ToList(); //add all the other scenes at the end
            i++;
        }

        var sceneArray = scenesForBuild.ToArray();
        EditorBuildSettings.scenes = sceneArray;

        BuildPipeline.BuildPlayer(sceneArray, Environment.GetCommandLineArgs().Last(), BuildTarget.WSAPlayer, BuildOptions.None);
    }
}