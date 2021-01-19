using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using GameBrains.Timers;
using UnityEngine;

namespace GameBrains.Minds
{
    public class Mind : MonoBehaviour
    {
        public Agent agent;
        protected virtual Agent Agent => agent;

        [SerializeField] protected float minimumTimeMs;
        [SerializeField] protected float maximumDelayMs;
        [SerializeField] protected RegulatorMode mode;
        [SerializeField] protected RegulatorDistribution regulatorDistribution;
        [SerializeField] protected AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        protected Regulator mindUpdateRegulator;
        public Regulator MindUpdateRegulator => mindUpdateRegulator;
        
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
            mindUpdateRegulator ??= new Regulator
            {
                MinimumTimeMs = minimumTimeMs,
                MaximumDelayMs = maximumDelayMs,
                Mode = mode,
                DelayDistribution = regulatorDistribution,
                DistributionCurve = distributionCurve
            };

            Agent.Mind = this;
        }

        // TODO: Make percept history available through a memory component??
        public virtual List<Action> Think(IEnumerable<Percept> percepts)
        {
            return null;
        }
    }
}