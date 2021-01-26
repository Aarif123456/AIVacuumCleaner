using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Entities.Agents;
using GameBrains.Percepts;
using UnityEngine;

namespace GameBrains.Minds
{
    public class SimpleReflexMind : SeekerMind
    {
        /* The efficiency at which we want to clean - NOTE: can't think of any reason why wouldn't 
        * try to maximize this ... 
        */
        [SerializeField] protected float desiredCleaningEfficiency = 1.0f;
        /* When do mark the tile as clean - we can experiment with different values */
        [SerializeField] protected float desiredCleanliness = 0.1f;
        public override List<Action> Think(IEnumerable<Percept> percepts)
        {
            /* Actions (that we can decide):
            * move to target block (Inherit from seekerMind?)
            * pick target block (based on what?)
            * Clean area ()
            */
            var actions=base.Think(percepts);
            /* We clean when we have a tile that is dirt and it has certain amount of dirt */
            if(ChooseCleanTile(percepts)){
                var cleanAction
                    = new CleanAction
                    {
                        cleaningEfficiency = this.desiredCleaningEfficiency,
                        desiredCleanliness = this.desiredCleanliness,
                        completionStatus = CompletionsStates.InProgress,
                        timeToLive = moveTimeToLive*1000
                    };
                actions.Add(cleanAction);
            }
            return actions;
        }

        protected virtual bool ChooseCleanTile(IEnumerable<Percept> percepts) {
            foreach (Percept percept in percepts)
            {
                if (percept is CleanPercept cleanPercept
                    && cleanPercept.Cleanable)
                {
                    return true;
                }
            }

            return false;
        } 
    }



}