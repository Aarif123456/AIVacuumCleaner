using GameBrains.Actions;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class SlerpLookAtActuator : Actuator
    {
        [SerializeField] protected float maximumAngularSpeed = 5f; // Represents a limitation of the actuator
        [SerializeField] protected float minimumSatisfactionAngle = 1f; // Represents a limitation of the actuator

        public override void Act(Action action)
        {
            if (action is ChangeDirectionAction changeDirectionAction)
            {
                Transform agentTransform = Agent.transform;
                float satisfactionAngle
                    = Mathf.Min(changeDirectionAction.desiredSatisfactionAngle, minimumSatisfactionAngle);
                float angularSpeed = Mathf.Min(changeDirectionAction.desiredAngularSpeed, maximumAngularSpeed);
                
                var angle = Vector3.Angle(agentTransform.forward, changeDirectionAction.desiredDirection);

                if (angle < satisfactionAngle)
                {
                    changeDirectionAction.completionStatus = CompletionsStates.Complete;
                }
                else
                {
                    agentTransform.rotation 
                        = Quaternion.Slerp(
                            agentTransform.rotation, 
                            Quaternion.LookRotation(changeDirectionAction.desiredDirection), 
                            angularSpeed * Time.deltaTime);
                
                    angle = Vector3.Angle(agentTransform.forward, changeDirectionAction.desiredDirection);
                
                    if (angle < satisfactionAngle)
                    {
                        changeDirectionAction.completionStatus = CompletionsStates.Complete;
                    }
                    else
                    {
                        changeDirectionAction.completionStatus = CompletionsStates.InProgress;
                    }
                }
            }
        }
    }
}