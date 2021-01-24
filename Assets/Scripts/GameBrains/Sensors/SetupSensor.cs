/* Set up all the sensors automatically */
using UnityEngine;
using GameBrains.Timers;
using System.Collections.Generic;

namespace GameBrains.Sensors
{
    public class SetupSensor : MonoBehaviour
    {

        public void Awake(){
            // Attach all the sensors to each object 
            GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
            
            for (int i=0; i<tiles.Length; i++){
                List<Sensor> sensors = new List<Sensor>();
                /* TODO add sensors */
                sensors.Add(gameObject.AddComponent<PositionSensor>() as PositionSensor);   
                sensors.Add(gameObject.AddComponent<CleaningSensor>() as CleaningSensor);  
                foreach(Sensor s in sensors){
                    s.AddGameObject(tiles[i]);
                    s.SetRegulatorMode(RegulatorMode.Normal);
                } 
            }
        }
    }
}