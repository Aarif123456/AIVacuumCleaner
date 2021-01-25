using GameBrains.Actions;
using GameBrains.Entities.Agents;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class MoveToPositionViaRigidBodyActuator : Actuator
    {
        // Represents a limitations of the actuator
        [SerializeField] protected float minimumSatisfactionRadius = 0.5f;
        [SerializeField] protected float maxForce = 5f;
        [SerializeField] protected float forceMultiplier = 0.1f;
        // Component needed to move using force                     
        [SerializeField] protected  Rigidbody rb;

        protected override void Start()
        {
            if (Agent.MotorType != MotorTypes.Rigidbody)
            {
                enabled = false;
                return;
            }
            base.Start();
            // The RigidBody component should be on the agent gameObject 
            if (rb == null) 
            {
                rb = Agent.GetComponent<Rigidbody>();
            }
        }

        public override void Act(Action action)
        {
            if (action is MoveToPositionAction moveToPositionAction)
            {
                Transform agentTransform = Agent.transform;
                Vector3 agentTransformPosition = agentTransform.position;
                float satisfactionRadius = minimumSatisfactionRadius;

                Vector3 desiredPosition = moveToPositionAction.desiredPosition;
                desiredPosition.y = agentTransformPosition.y;
                if (Vector3.Distance(agentTransformPosition,desiredPosition) <= satisfactionRadius)
                {
                    moveToPositionAction.completionStatus = CompletionsStates.Complete;
                }
                else{
                    /*go toward the desired position*/
                    Vector3 desiredDirection = (desiredPosition - agentTransformPosition).normalized;

                    /*try to reach desired speed */
                    Vector3 desiredVelocity = desiredDirection * moveToPositionAction.desiredSpeed;

                    /*calculate force based on our current velocity an desired velocity and our 
                    force multiplier */
                    Vector3 force = (desiredVelocity - rb.velocity) * forceMultiplier;

                    /*If we reach maximum thrust cap the acceleration */
                    if (force.magnitude > maxForce)
                    {
                        force = force.normalized * maxForce;
                    }

                    /* Add force to rigid-body */
                    rb.AddForce(force);
                    if (Vector3.Distance(agentTransformPosition,desiredPosition) <= satisfactionRadius) 
                    {
                        moveToPositionAction.completionStatus = CompletionsStates.Complete;
                    } 
                    else 
                    {
                        moveToPositionAction.completionStatus = CompletionsStates.InProgress;
                    }
                }
            }
        }
    }
}