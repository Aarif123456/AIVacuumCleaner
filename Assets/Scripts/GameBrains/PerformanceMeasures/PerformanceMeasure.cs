using GameBrains.Entities.Agents;
using UnityEngine;

namespace GameBrains.PerformanceMeasures
{
    public class PerformanceMeasure : MonoBehaviour
    {
        [SerializeField] protected Agent agent;
        protected virtual Agent Agent => agent;

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

        // TODO: Measure Performance
        /*TODO: Measure dirt collected, energy used, */
    }
}