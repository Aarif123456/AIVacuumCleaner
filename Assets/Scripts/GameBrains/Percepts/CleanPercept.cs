using UnityEngine;

namespace GameBrains.Percepts
{
    public class CleanPercept : Percept
    {
        public float DirtInArea{ get; set; }
        public float Dirtiness{ get; set; }
        public bool Cleanable{ get; set; }
    }
}