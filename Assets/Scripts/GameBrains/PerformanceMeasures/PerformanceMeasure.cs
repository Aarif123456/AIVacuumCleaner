using GameBrains.Entities.Agents;
using UnityEngine;

namespace GameBrains.PerformanceMeasures
{
    public class PerformanceMeasure : MonoBehaviour
    {
        [SerializeField] protected Agent agent;
        [SerializeField] protected float dirtSucked = 0f;
        [SerializeField] protected float energyUsed = 0f;
        [SerializeField] public float currentEfficiency = 0f;

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

        protected virtual void Update(){
            if(energyUsed>0f){
                currentEfficiency = dirtSucked/energyUsed;
            }
            Debug.Log("Current Efficiency " + currentEfficiency);
        }

        public void AddSuction(float dirt, float energy){
            dirtSucked += dirt;
            energyUsed += energy;
        }
    }
}