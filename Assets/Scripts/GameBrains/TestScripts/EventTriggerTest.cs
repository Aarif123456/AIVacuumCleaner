using GameBrains.EventManagers;
using UnityEngine;

namespace GameBrains.TestScripts
{
    public class EventTriggerTest : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                EventManager.TriggerEvent("test", 1);
            }
        }
    }
}