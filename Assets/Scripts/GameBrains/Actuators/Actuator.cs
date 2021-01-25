using System.Collections.Generic;
using GameBrains.Entities.Agents;
using GameBrains.Actions;
using GameBrains.Timers;
using UnityEngine;

/* Actuator class will control how the agent will execute different actions */
namespace GameBrains.Actuators
{
    public class Actuator : MonoBehaviour
    {
        [SerializeField] protected Agent agent;
        protected virtual Agent Agent => agent;
        
        [SerializeField] protected float minimumTimeMs;
        [SerializeField] protected float maximumDelayMs;
        [SerializeField] protected RegulatorMode mode;
        [SerializeField] protected RegulatorDistribution regulatorDistribution;
        [SerializeField] protected AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        protected Regulator actuatorUpdateRegulator;
        public Regulator ActuatorUpdateRegulator => actuatorUpdateRegulator;

        protected virtual void Awake()
        {
            /* The Agent component should either be attached to the same
            gameObject as the Actuator component or above it in the hierarchy.
            This checks the gameObject first and then works its way upward.*/
            if (agent == null)
            {
                agent = GetComponentInParent<Agent>();
            }
        }

        protected virtual void Start()
        {
            actuatorUpdateRegulator ??= new Regulator
            {
                MinimumTimeMs = minimumTimeMs,
                MaximumDelayMs = maximumDelayMs,
                Mode = mode,
                DelayDistribution = regulatorDistribution,
                DistributionCurve = distributionCurve
            };
            
            Agent.Actuators.Add(this);
        }
    
        public virtual void Act(List<Action> actions)
        {
            foreach (Action action in actions)
            {
                Act(action);
            }
        }

        public virtual void Act(Action action)
        {
            Debug.LogWarning("WARNING: Using act method from base actuator class" );
        }
    }
}