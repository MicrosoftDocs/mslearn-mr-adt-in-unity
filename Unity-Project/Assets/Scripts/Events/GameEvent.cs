// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Scriptable Object used to pass events between the scene and UI
/// </summary>
[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/Events/Game Event")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> listeners =
        new List<GameEventListener>();

    public void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnEventRaised();
        }
    }

    public void RegisterListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}

#if UNITY_EDITOR

[UnityEditor.CustomEditor(typeof(GameEvent))]
public class GameEventEditor : UnityEditor.Editor
{
    private static bool showDebug;

    private GameEvent GameEvent => (GameEvent)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(5);

        showDebug = UnityEditor.EditorGUILayout.Foldout(showDebug, "Debug Tools [Editor Playmode Only]");

        if (!showDebug)
            return;

        using (new UnityEditor.EditorGUI.DisabledScope(!Application.isPlaying))
        {
            if (GUILayout.Button("Manually Trigger Event"))
            {
                GameEvent.Raise();
            }
        }
    }
}

#endif