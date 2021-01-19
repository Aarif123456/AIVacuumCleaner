using GameBrains.Percepts;
using UnityEngine;

namespace GameBrains.Sensors
{
    public class PositionSensor : Sensor
    {
        [SerializeField] float sensorRange = 20.0f;
        [SerializeField] Transform targetTransform;

        public override Percept Sense()
        {
            PositionPercept positionPercept = new PositionPercept();
            var agentPosition = Agent.transform.position;
            var targetPosition = targetTransform.position;

            if (targetTransform != null &&
                Vector3.Distance(agentPosition, targetPosition) <= sensorRange)
            {
                positionPercept.Position = targetPosition;
            }
            else
            {
                positionPercept.Position = null;
            }

            return positionPercept;
        }
    }
}