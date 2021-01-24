using UnityEngine;
using GameBrains.Entities;

namespace GameBrains.Actions
{
    public class CleanAction : Action
    {
        /* TODO: should this be handled in the actuator? */
        public CleanableEntity tile;
        public float cleaningEfficiency;
        public float desiredCleanliness;
    }
}