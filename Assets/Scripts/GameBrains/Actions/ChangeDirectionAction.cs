using UnityEngine;

namespace GameBrains.Actions
{
    public class ChangeDirectionAction : Action
    {
        public Vector3 desiredDirection;

        // TODO: Should these be part of the action or set via a different action
        public float desiredAngularSpeed;
        public float desiredSatisfactionAngle;
    }
}