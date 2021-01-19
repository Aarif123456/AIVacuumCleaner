using UnityEngine;

namespace GameBrains.Actions
{
    public class ChangeVelocityAction : Action
    {
        public Vector3 desiredVelocity;
        public Rigidbody rb;

        void Start()
           {
               rb = GetComponent<Rigidbody>();
           }

        // TODO: Do we want to limit the rate of change of the velocity via this action??
        public Vector3 desiredMaximumAcceleration;
    }
}