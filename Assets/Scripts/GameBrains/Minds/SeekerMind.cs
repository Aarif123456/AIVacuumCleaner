using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using UnityEngine;

namespace GameBrains.Minds
{
    public class SeekerMind : Mind
    {
        // TODO: Make parameters settable through Mind decisions??
        
        // How close is close enough?
        [SerializeField] protected float desiredSatisfactionRadius = 0.5f; // TODO: Should depend on Agent radius??
        // How fast should we move?
        [SerializeField] protected float desiredSpeed = 100f; // TODO: Should depend on Actuator capabilities??
        [SerializeField] protected float moveTimeToLive = 5f;

        [SerializeField] protected float desiredSatisfactionAngle = 2; // TODO: Should depend on Actuator capabilities??
        [SerializeField] protected float desiredAngularSpeed = 1f; // TODO: Should depend on Actuator capabilities??
        [SerializeField] protected float turnTimeToLive = 5f;



        public override List<Action> Think(IEnumerable<Percept> percepts)
        {
            Vector3 targetPosition;
            List<Action> actions = new List<Action>();
            Transform agentTransform = Agent.transform;
            Vector3 agentPosition = agentTransform.position;
            targetPosition = ChooseTargetPosition(percepts);

            // TODO: If Actuator cannot achieve desire this keeps trying endlessly??
            if (Vector3.Distance(agentPosition, targetPosition) > desiredSatisfactionRadius)
            {
                var moveToPositionAction
                    = new MoveToPositionAction
                    {
                        desiredPosition = targetPosition,
                        desiredSpeed = desiredSpeed,
                        desiredSatisfactionRadius = desiredSatisfactionRadius,
                        completionStatus = CompletionsStates.InProgress,
                        timeToLive = moveTimeToLive
                    };
                actions.Add(moveToPositionAction);
            }

            Vector3 desiredDirection = (targetPosition - agentPosition).normalized;
            var angle = Vector3.Angle(agentTransform.forward, desiredDirection);

            if (angle > desiredSatisfactionAngle)
            {
                var changeDirectionAction
                    = new ChangeDirectionAction
                    {
                        desiredDirection = desiredDirection,
                        desiredAngularSpeed = desiredAngularSpeed,
                        desiredSatisfactionAngle = desiredSatisfactionAngle,
                        completionStatus = CompletionsStates.InProgress,
                        timeToLive = turnTimeToLive
                    };
                actions.Add(changeDirectionAction);
            }

            return actions;
        }

        protected Vector3 ChooseTargetPosition(IEnumerable<Percept> percepts)
        {
            switch (Agent.TargetType)
            {
                // TODO: prioritize targets
                case TargetTypes.None:
                    return Agent.transform.position;
                case TargetTypes.First:
                    return ChooseFirstTargetPosition(percepts);
                case TargetTypes.Closest:
                    return ChooseClosestTargetPosition(percepts);
                case TargetTypes.Valued:
                    Debug.LogWarning("TargetType.Valued is not implemented. Defaulting to Random.");
                    return ChooseRandomTargetPosition(percepts);
                default:
                    return ChooseRandomTargetPosition(percepts);
            }
        }

        protected Vector3 ChooseRandomTargetPosition(IEnumerable<Percept> percepts)
        {
            List<Vector3> targetPositions = new List<Vector3>();

            foreach (Percept percept in percepts)
            {
                if (percept is PositionPercept positionPercept
                    && positionPercept.Position.HasValue)
                {
                    targetPositions.Add(positionPercept.Position.Value);
                }
            }

            if (targetPositions.Count > 0)
            {
                var randomIndex = Random.Range(0, targetPositions.Count);

                print("LookAt = " + randomIndex);

                return targetPositions[randomIndex];
            }
            
            return Agent.transform.position; // no target
        }

        protected Vector3 ChooseFirstTargetPosition(IEnumerable<Percept> percepts)
        {
            foreach (Percept percept in percepts)
            {
                if (percept is PositionPercept positionPercept
                    && positionPercept.Position.HasValue)
                {
                    return positionPercept.Position.Value;
                }
            }

            return Agent.transform.position; // no target
        }
        
        protected Vector3 ChooseClosestTargetPosition(IEnumerable<Percept> percepts)
        {
            List<Vector3> targetPositions = new List<Vector3>();

            foreach (Percept percept in percepts)
            {
                if (percept is PositionPercept positionPercept
                    && positionPercept.Position.HasValue)
                {
                    targetPositions.Add(positionPercept.Position.Value);
                }
            }

            int closestIndex = -1;
            float closestDistance = float.PositiveInfinity;

            for (int i = 0; i < targetPositions.Count; i++)
            {
                float distance 
                    = Vector3.Distance(Agent.transform.position, targetPositions[i]);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            if (closestIndex != -1)
            {
                return targetPositions[closestIndex];
            }

            return Agent.transform.position; // no target
        }
    }
}