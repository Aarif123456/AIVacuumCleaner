using UnityEngine;

namespace GameBrains.Actions
{
    public class ChangeDirectionAction : Action
    {
        public Vector3 desiredDirection;

        // NOTE: we could handle changing direction with the move to position action
        public float desiredAngularSpeed;
    }
}