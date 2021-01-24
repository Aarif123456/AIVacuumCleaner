using GameBrains.Percepts;
using GameBrains.Entities;
using UnityEngine;

namespace GameBrains.Sensors
{
    public class CleaningSensor : Sensor
    {
        [SerializeField] float sensorRange = 20.0f;
        [SerializeField] CleanableEntity cleanableEntity;
        [SerializeField] Transform targetTransform;
        
        public override void AddGameObject(GameObject g)
        {
            cleanableEntity = g.GetComponent<CleanableEntity>();
            targetTransform = g.GetComponent<Transform>();
        }

        public override Percept Sense()
        {
            var cleanPercept = new CleanPercept();
            var agentPosition = Agent.transform.position;
            var targetPosition = targetTransform.position;
            
            if (targetTransform != null &&
                Vector3.Distance(agentPosition, targetPosition) <= sensorRange)
            {
                cleanPercept.Cleanable = true;
                cleanPercept.DirtInArea = cleanableEntity.GetTotalDirt();
            }

            return cleanPercept;
        }
    }
}