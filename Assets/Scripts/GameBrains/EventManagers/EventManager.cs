using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameBrains.EventManagers
{
    [System.Serializable]
    public class MyEvent : UnityEvent<object>
    {
    }

    public class EventManager : MonoBehaviour
    {
        private Dictionary<string, MyEvent> eventDictionary;

        private static EventManager eventManager;

        public static EventManager Singleton
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType<EventManager>();

                    if (!eventManager)
                    {
                        Debug.LogError ("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Initialize(); 
                    }
                }

                return eventManager;
            }
        }

        void Initialize()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, MyEvent>();
            }
        }

        public static void StartListening (string eventName, UnityAction<object> listener)
        {
            if (Singleton.eventDictionary.TryGetValue(eventName, out var thisEvent))
            {
                thisEvent.AddListener(listener);
            } 
            else
            {
                thisEvent = new MyEvent();
                thisEvent.AddListener(listener);
                Singleton.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, UnityAction<object> listener)
        {
            if (eventManager == null)
            {
                return;
            }

            MyEvent thisEvent = null;

            if (Singleton.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent(string eventName, object eventData)
        {
            MyEvent thisEvent = null;

            if (Singleton.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventData);
            }
        }
    }
}