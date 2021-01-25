using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Entities;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class CleaningActuator : Actuator
    {
        // Represents a limitations of the actuator
        [SerializeField] protected float baseSuctionEfficiency = 0.25f;
        [SerializeField] protected float maxSuctionEfficiency = 0.4f;
        [SerializeField] protected float baseSuctionEnergyConsumption = 100.0f;
        [SerializeField] protected float maxSuctionEnergyConsumption = 200.0f;
        /* The maximum amount of dirt the robot can suck */
        [SerializeField] protected float maxDirtPerSecond = 220.0f;
        /* Bit shift the index of the layer (8) to get a bit mask
        * This would cast rays only against colliders in layer 8.
        * But instead we want to collide against everything except layer 8. 
        * The ~ operator does this, it inverts a bitmask.
        */
        [SerializeField] protected int layerMask = ~(1 << 8);
        [SerializeField] protected RaycastHit hit;

        protected override void Start()
        {
            base.Start();
        }

        public override void Act(Action action)
        {
            if (action is CleanAction cleanAction)
            {
                Transform agentTransform = Agent.transform;
                var agentPos = agentTransform.position + new Vector3(0f,1f, 0f);

                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(agentPos, agentTransform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layerMask))
                {
                    /* TODO  - clean till area clean - and go to turbo if performances increases */
                    Debug.DrawRay(agentPos, agentTransform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
                    if(hit.collider.tag == "Tile"){
                        var dirtCleaned = hit.collider.gameObject.GetComponent<CleanableEntity>().CleanArea( baseSuctionEfficiency, maxDirtPerSecond * Time.deltaTime, 0);
                        /* TODO: store measures in performance measure */
                    }
                }
                else
                {
                    Debug.DrawRay(agentPos, agentTransform.TransformDirection(Vector3.down) * 1000, Color.white);
                    Debug.LogWarning("Bot is not above any cleanable tile");
                }
            }
        }
    }
}