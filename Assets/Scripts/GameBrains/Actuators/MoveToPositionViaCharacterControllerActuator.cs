﻿using GameBrains.Actions;
using GameBrains.Entities.Agents;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class MoveToPositionViaCharacterControllerActuator : Actuator
    {
        [SerializeField] protected float minimumSatisfactionRadius = 0.5f;    
        [SerializeField] protected float maximumSpeed = 1f;                    
        [SerializeField] protected CharacterController characterController;

        protected override void Start()
        {
            if (Agent.MotorType != MotorTypes.CharacterController)
            {
                enabled = false;
                return;
            }
            
            base.Start();
            
            // The CharacterController component should be attached to the same gameObject as the Agent component.
            if (characterController == null) 
            {
                characterController = Agent.GetComponent<CharacterController>();
            }
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
                
                if (Vector3.Distance(agentTransformPosition,desiredPosition) <= satisfactionRadius)
                {
                    moveToPositionAction.completionStatus = CompletionsStates.Complete;
                } 
                else
                {
                    Vector3 desiredDirection = (desiredPosition - agentTransformPosition).normalized;
                    Vector3 desiredVelocity = desiredDirection * speed;

                    //characterController.SimpleMove(desiredVelocity);
                    // TODO: Need to handle gravity for Move
                    characterController.Move(desiredVelocity * Time.deltaTime);

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