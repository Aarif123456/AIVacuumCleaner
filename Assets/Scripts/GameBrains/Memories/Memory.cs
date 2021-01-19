using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using GameBrains.Timers;
using UnityEngine;

// TODO: How should we store state for agents types that have state memory??

namespace GameBrains.Memories
{
    public class Memory : MonoBehaviour
    {
        [SerializeField] protected Agent agent;
        protected virtual Agent Agent => agent;
        
        [SerializeField] protected float minimumTimeMs;
        [SerializeField] protected float maximumDelayMs;
        [SerializeField] protected RegulatorMode mode;
        [SerializeField] protected RegulatorDistribution regulatorDistribution;
        [SerializeField] protected AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        protected Regulator memoryUpdateRegulator;
        public Regulator MemoryUpdateRegulator => memoryUpdateRegulator;
        
        public virtual List<Percept> CurrentPercepts { get; protected set; }

        public virtual List<Action> CurrentActions { get; protected set; }

        protected virtual void Awake()
        {
            // The Agent component should either be attached to the same
            // gameObject as the Actuator component or above it in the hierarchy.
            // This checks the gameObject first and then works its way upward.
            if (agent == null)
            {
                agent = GetComponentInParent<Agent>();
            }
        }
        public virtual void Start()
        {
            memoryUpdateRegulator ??= new Regulator
            {
                MinimumTimeMs = minimumTimeMs,
                MaximumDelayMs = maximumDelayMs,
                Mode = mode,
                DelayDistribution = regulatorDistribution,
                DistributionCurve = distributionCurve
            };

            Agent.Memory = this;
        }

        public virtual void Record(List<Percept> percepts)
        {
            CurrentPercepts = percepts;
        }

        public virtual void Record(List<Action> actions)
        {
            CurrentActions = actions;
        }
    }
}