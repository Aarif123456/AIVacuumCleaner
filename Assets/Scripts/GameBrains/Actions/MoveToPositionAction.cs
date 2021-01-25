using UnityEngine;

namespace GameBrains.Actions
{
    public class MoveToPositionAction : Action
    {
        public Vector3 desiredPosition;

        /* NOTE: the desired speed is given through the action but the actual speed is dependent
        * on the actuator
        */ 
        public float desiredSpeed;
    }
}