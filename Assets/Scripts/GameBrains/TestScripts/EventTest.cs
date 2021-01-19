using GameBrains.EventManagers;
using UnityEngine;
using UnityEngine.Events;

namespace GameBrains.TestScripts
{
    public class EventTest : MonoBehaviour
    {
        private UnityAction<object> someListener;

        void Awake ()
        {
            someListener = SomeFunction;
        }

        void OnEnable ()
        {
            EventManager.StartListening ("test", someListener);
        }

        void OnDisable ()
        {
            EventManager.StopListening ("test", someListener);
        }

        void SomeFunction(object eventData)
        {
            int x = (int)eventData;
            Debug.Log ("Some Function was called! " + x);
        }
    }
} 