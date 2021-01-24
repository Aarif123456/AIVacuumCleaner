
using UnityEngine;
namespace GameBrains.Entities
{
    public class CleanableEntity : TargetEntity
    {

        public static readonly  Color DIRT_COLOR = new Vector4(0.745f, 0.494f, 0.243f);
        // The max speed at which the location gets dirty per second
        public float dirtAccumulationRate=20.0f; 
        /* A value from 0.0-1.0 that represent the how consistent that accumulation of dirt 
        * where 1 means the rate is constant
        * e.g if the rate variance is 50% and the dirt increase is 30/s then every second 
        * we can add dirt ranging from 15-30
        */
        public float dirtAccumulationVariance=1.0f; 
        // how much the dirt the current location has
        public volatile float currentDirt=0;
        // The maximum amount of dirt the area can accumulate
        public int maxDirtLevel=2147483647;
        /* How hard the location is to clean where 1 means it cannot be clean and 0 means that 
        * it has no resistance this represents the fact that some places like carpets will be harder
        *  to clean then hardwood floors
        */
        public float resistanceRate;
        [SerializeField] protected Material tileMaterial ;

        public override void Awake()
        {
            //Fetch the Material from the Renderer of the GameObject
            tileMaterial = this.gameObject.GetComponent<Renderer>().material;
            // make sure rendering is on to have feedback
            this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            maxDirtLevel = Mathf.Max(maxDirtLevel, 1);
        }
        
        public override void Update()
        {
            base.Update();
            float maxDirtIncrease = dirtAccumulationRate*Time.deltaTime;
            currentDirt += Random.Range(maxDirtIncrease*dirtAccumulationVariance, maxDirtIncrease);
            currentDirt = Mathf.Min(maxDirtLevel, currentDirt);
            SetAreaColor();
        }

        public void SetAreaColor(){
            if(tileMaterial != null){
                // first we turn the amount of dirt percentage into a value
                float dirtiness = (currentDirt/maxDirtLevel);
                // Next we darken the color depending on how dirty the tile is
                float r = Mathf.Min(1f, DIRT_COLOR.r - dirtiness);
                float g = Mathf.Min(1f, DIRT_COLOR.g - dirtiness);
                float b = Mathf.Min(1f, DIRT_COLOR.b - dirtiness);
                float a = DIRT_COLOR.a;
                tileMaterial.color = new Color(r, g, b, a);
            } 
            else{
                Debug.LogWarning("Warning: unable to find material on tile... Color cannot be set");
            }
        }

        /* clean the are and get a certain amount of dirt */
        public void CleanArea(float dirtCleaned){
            currentDirt-=dirtCleaned;
        }

        /* Return amount of dirt based on percentage passed in the resistance of the area */
        public float GetTotalDirtByPercentage(float percentage){
            return Mathf.Max(0, currentDirt * (percentage-resistanceRate));
        }

        public float GetTotalDirt(){
            return currentDirt;
        }
    }
}