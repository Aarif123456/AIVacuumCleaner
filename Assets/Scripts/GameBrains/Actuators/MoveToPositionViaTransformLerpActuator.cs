using GameBrains.Actions;
using GameBrains.Entities.Agents;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class MoveToPositionViaTransformLerpActuator : Actuator
    {
        // Represents limitations of the actuator
        [SerializeField] protected float minimumSatisfactionRadius = 0.5f;    
        [SerializeField] protected float maximumSpeed = 2f;                    

        protected override void Start()
        {
            if (Agent.MotorType != MotorTypes.TransformLerp)
            {
                enabled = false;
                return;
            }
            
            base.Start();
        }

        public override void Act(Action action)
        {
            if (action is MoveToPositionAction moveToPositionAction)
            {
                Transform agentTransform = Agent.transform;
                Vector3 agentTransformPosition = agentTransform.position;
                float satisfactionRadius = minimumSatisfactionRadius;
                float speed = Mathf.Min(moveToPositionAction.desiredSpeed, maximumSpeed);
                
                Vector3 desiredPosition = moveToPositionAction.desiredPosition;
                desiredPosition.y = agentTransformPosition.y;
                
                // TODO: handle gravity??
                
                if (Vector3.Distance(agentTransformPosition,desiredPosition) <= satisfactionRadius)
                {
                    moveToPositionAction.completionStatus = CompletionsStates.Complete;
                }
                else
                {
                    agentTransform.position
                        = Vector3.Lerp(agentTransformPosition,desiredPosition,speed * Time.deltaTime);

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