// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using BladeMR.Events;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class ADTAuthenticationEventListener : MonoBehaviour
    {
        [SerializeField]
        private ADTAuthenticationGameEvent gameEvent;

        [SerializeField]
        public ADTAuthenticationUnityEvent response;

        private void OnEnable()
        {
            gameEvent.RegisterListener(this);
        }

        private void OnDisable()
        {
            gameEvent.UnregisterListener(this);
        }

        public void OnEventRaised(string eventData)
        {
            response.Invoke(eventData);
        }
    }
}
