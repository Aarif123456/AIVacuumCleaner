using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using UnityEngine;

/* Mind that figures out where and how to move to a target block */
namespace GameBrains.Minds
{
    public class SeekerMind : Mind
    {
        // TODO: Make parameters settable through Mind decisions??
        
        // How close is close enough?
        [SerializeField] protected float desiredSatisfactionRadius = 0f; 
        // How fast should we want move? NOTE: actual speed is dependent on actuator 
        [SerializeField] protected float desiredSpeed = 100f; 
        [SerializeField] protected float moveTimeToLive = 5f;
        // NOTE: actual precision is dependent on actuator capability 
        [SerializeField] protected float desiredSatisfactionAngle = 2; 
        [SerializeField] protected float desiredAngularSpeed = 1f; 
        [SerializeField] protected float turnTimeToLive = 5f;
        
        /* We want to get the scale of the object relative to the world - but unity only gives
        * the local scale:
        * solution from: https://forum.unity.com/threads/world-scale.2270/
        */
        protected Vector3 GetWorldScale(Transform transform)
        {
            Vector3 worldScale = transform.localScale;
            Transform parent = transform.parent;
           
            while (parent != null)
            {
                worldScale = Vector3.Scale(worldScale,parent.localScale);
                parent = parent.parent;
            }
           
            return worldScale;
        }

        protected override void Awake()
        {
            base.Awake();
            /* NOTE: we are using tags to avoid manually setting up the script
            *   However, this makes it so the objects must be tagged correctly 
            */
            var agentObject = GameObject.FindWithTag("Player");
            var agentMesh = agentObject.GetComponent<MeshRenderer>();
            var agentObjectTransform = GetWorldScale(agentObject.GetComponent<Transform>());
            Bounds bounds = agentMesh.bounds;
            /*NOTE: we use the minimum of the two bounds because we don't want to constraint 
            * the shape of the agent 
            * we only look at the bounds for x and z because our agent only moves along the x
            *   and z axis. It might make sense to have a separate radius for the x and z
            * instead of combining them...
            */
            desiredSatisfactionRadius = Mathf.Max(bounds.extents.x * agentObjectTransform.x,
                                                    bounds.extents.z * agentObjectTransform.z);

        }

        /* Bot chooses target depending on mode and then goes to the target */
        public override List<Action> Think(IEnumerable<Percept> percepts)
        {
            Vector3 targetPosition;
            List<Action> actions = new List<Action>();
            Transform agentTransform = Agent.transform;
            Vector3 agentPosition = agentTransform.position;
            targetPosition = ChooseTargetPosition(percepts);
            
            // NOTE: If Actuator cannot achieve desire it will keeps trying endlessly
            if (Vector3.Distance(agentPosition, targetPosition) > desiredSatisfactionRadius)
            {
                var moveToPositionAction
                    = new MoveToPositionAction
                    {
                        desiredPosition = targetPosition,
                        desiredSpeed = desiredSpeed,
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
                case TargetTypes.None:
                    return Agent.transform.position;
                case TargetTypes.First:
                    return ChooseFirstTargetPosition(percepts);
                case TargetTypes.Closest:
                    return ChooseClosestTargetPosition(percepts);
                case TargetTypes.Valued:
                    return ChooseValuedTargetPosition(percepts);
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

        /* NOTE: Currently the most dirty is the most valuable */
        protected Vector3 ChooseValuedTargetPosition(IEnumerable<Percept> percepts)
        {
            var targetValues = new List<float>();
            List<Vector3> targetPositions = new List<Vector3>();

            foreach (Percept percept in percepts)
            {
                if (percept is CleanPercept cleanPercept
                    && cleanPercept.Cleanable)
                {
                    targetValues.Add(cleanPercept.DirtInArea);
                }
                if (percept is PositionPercept positionPercept
                    && positionPercept.Position.HasValue)
                {
                    targetPositions.Add(positionPercept.Position.Value);
                }
            }

            /* NOTE: we are assuming that the cleaning sensor and position sensor will always 
            * create a related list - This may not always be the case depending on the implementation
            * TODO: how can we aggregate info from our sensors better?
            */
            if(targetValues.Count != targetPositions.Count){
                Debug.LogWarning("Number of cleanable location do not match tiles in sensor");
                return Agent.transform.position; // no target
            }
            int valueIndex = -1;
            float tileValue = float.NegativeInfinity;

            for (int i = 0; i < targetPositions.Count; i++)
            {

                if (targetValues[i] > tileValue)
                {
                    tileValue = targetValues[i];
                    valueIndex = i;
                }
            }

            Debug.Log("valuable targetPos" + targetPositions[valueIndex] );
            if (valueIndex != -1)
            {
                return targetPositions[valueIndex];
            }

            return Agent.transform.position; // no target
        }
    }
}