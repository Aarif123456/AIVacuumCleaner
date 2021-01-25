using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Entities;
using UnityEngine;

namespace GameBrains.Actuators
{
    public class CleaningActuator : Actuator
    {
        /* We will use ray casting to ensure that we only clean entities directly below us */
        [SerializeField] protected RaycastHit hit;

        // Represents limitations of the actuator
        [SerializeField] protected float baseSuctionEfficiency = 0.25f;
        [SerializeField] protected float maxSuctionEfficiency = 0.4f;
        [SerializeField] protected float baseSuctionEnergyConsumption = 100.0f;
        [SerializeField] protected float maxSuctionEnergyConsumption = 200.0f;
        /* The maximum amount of dirt the robot can suck */
        [SerializeField] protected float maxDirtPerSecond = 220.0f;
        
        

        protected override void Start()
        {
            base.Start();
        }

        public override void Act(Action action)
        {
            if (action is CleanAction cleanAction)
            {
                // Does the ray intersect any objects excluding the player layer
                if (TileBelow())
                {
                    /* TODO  - clean till area clean - and go to turbo if performances increases */
                    if(hit.collider.tag == "Tile"){
                        var dirtCleaned = hit.collider.gameObject.GetComponent<CleanableEntity>().CleanArea( baseSuctionEfficiency, maxDirtPerSecond * Time.deltaTime, 0);
                        /* TODO: store measures in performance measure */
                    } else{
                        Debug.LogWarning("Ray cast did not hit tile, instead it hit a " + hit.collider.tag);
                    }
                }
                else
                {
                    Debug.LogWarning("Bot is not above any cleanable tile:" + hit.collider.tag);
                }
            }
        }

        public bool TileBelow(){
            Transform agentTransform = Agent.transform;
            /* We offset the vector */
            var agentPos = agentTransform.position + new Vector3(0f, 10f, 0f);
            Debug.DrawRay(agentPos, agentTransform.TransformDirection(Vector3.down) * 1000, Color.yellow);
            return Physics.Raycast(agentPos, agentTransform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
        }
    }
}