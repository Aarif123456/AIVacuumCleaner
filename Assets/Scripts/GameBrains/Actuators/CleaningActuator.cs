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
        [SerializeField] protected float minDirtPerSecond = 0f;
        [SerializeField] protected bool turbo = false;

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
                    if(hit.collider.tag == "Tile"){
                        var area = hit.collider.gameObject.GetComponent<CleanableEntity>();
                        var preDirtiness = area.GetDirtiness();
                        var dirtCleaned = CleanArea ( area );
                        var postDirtiness = area.GetDirtiness();

                        /* TODO: store measures in performance measure */

                        ToggleTurbo(preDirtiness, postDirtiness,  dirtCleaned, cleanAction);
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

        protected void ToggleTurbo(float preDirtiness, float postDirtiness, float dirtCleaned, CleanAction cleanAction){
            if(postDirtiness <= cleanAction.desiredCleanliness){
                cleanAction.completionStatus = CompletionsStates.Complete;
                turbo = false;
            }
            else
            {
                cleanAction.completionStatus = CompletionsStates.InProgress;
            }
            /* TODO: find a better cutoff point for going turbo */
            /* Go turbo when we know we have dirt but we didn't collect any dirt */
            if(dirtCleaned == minDirtPerSecond|| preDirtiness > postDirtiness){
                if(!turbo){
                    Debug.LogWarning("Going Turbo !!!!!!!!!");
                }
                turbo = true;
            }
        }
        public bool TileBelow(){
            Transform agentTransform = Agent.transform;
            /* We offset the vector */
            var agentPos = agentTransform.position + new Vector3(0f, 10f, 0f);
            var gotRayHit = Physics.Raycast(agentPos, agentTransform.TransformDirection(Vector3.down), out hit, Mathf.Infinity);
            Debug.DrawRay(agentPos, agentTransform.TransformDirection(Vector3.down) * 1000, (gotRayHit ? Color.yellow : Color.white) );
            return gotRayHit;
        }

        public float CleanArea (CleanableEntity area){
            /* Get the suction rate and energy used depending on if we are in turbo mode */
            var suctionRate = baseSuctionEfficiency;
            var energyConsumptionRate = baseSuctionEnergyConsumption;
            if(turbo) {
                Debug.LogWarning("Using Turbo setting");
                suctionRate = maxSuctionEfficiency;
                energyConsumptionRate = maxSuctionEnergyConsumption;
            }
            /* Dirtiness level before cleaning */
            var dirtCleaned = area.CleanArea( suctionRate, maxDirtPerSecond, minDirtPerSecond );
            Debug.Log("dirt cleaned" + dirtCleaned);
            return dirtCleaned;
        }
    }
}