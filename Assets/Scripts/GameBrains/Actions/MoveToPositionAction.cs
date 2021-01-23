using UnityEngine;

namespace GameBrains.Actions
{
    public class MoveToPositionAction : Action
    {
        public Vector3 desiredPosition;

        // TODO: Should these parameters be set via this action or another action??
        public float desiredSpeed;
        public float desiredSatisfactionRadius;
    }
}