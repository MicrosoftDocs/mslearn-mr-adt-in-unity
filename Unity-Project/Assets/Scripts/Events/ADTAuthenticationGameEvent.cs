// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BladeMR.Events
{
    [CreateAssetMenu(fileName = "ADTAuthenticationGameEvent", menuName = "Scriptable Objects/Events/ADT Authentication Event")]
    public class ADTAuthenticationGameEvent : ScriptableObject
    {
        private List<ADTAuthenticationEventListener> listeners =
            new List<ADTAuthenticationEventListener>();

        public void Raise(string eventData)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(eventData);
            }
        }

        public void RegisterListener(ADTAuthenticationEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(ADTAuthenticationEventListener listener)
        {
            listeners.Remove(listener);
        }
    }

    [Serializable]
    public class ADTAuthenticationUnityEvent : UnityEvent<string> { }
}