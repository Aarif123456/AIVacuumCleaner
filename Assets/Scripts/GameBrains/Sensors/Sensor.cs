using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using GameBrains.Timers;
using UnityEngine;

namespace GameBrains.Sensors
{
    public class Sensor : MonoBehaviour
    {
        [SerializeField] protected Agent agent;
        protected virtual Agent Agent => agent;

        [SerializeField] protected float minimumTimeMs;
        [SerializeField] protected float maximumDelayMs;
        [SerializeField] protected RegulatorMode mode;
        [SerializeField] protected RegulatorDistribution regulatorDistribution;
        [SerializeField] protected AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        
        protected Regulator sensorUpdateRegulator;
        public Regulator SensorUpdateRegulator => sensorUpdateRegulator;

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
            sensorUpdateRegulator ??= new Regulator
            {
                MinimumTimeMs = minimumTimeMs,
                MaximumDelayMs = maximumDelayMs,
                Mode = mode,
                DelayDistribution = regulatorDistribution,
                DistributionCurve = distributionCurve
            };
            
            Agent.Sensors.Add(this);
        }

        public virtual Percept Sense()
        {
            return null;
        }
    }
}