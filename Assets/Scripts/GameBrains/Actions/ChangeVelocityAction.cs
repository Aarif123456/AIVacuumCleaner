using UnityEngine;

namespace GameBrains.Actions
{
    public class ChangeVelocityAction : Action
    {
        public Vector3 desiredVelocity;

        // TODO: Do we want to limit the rate of change of the velocity via this action??
        public Vector3 desiredMaximumAcceleration;
    }
}