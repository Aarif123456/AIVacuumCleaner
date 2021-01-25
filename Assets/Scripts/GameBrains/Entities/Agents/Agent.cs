using System.Collections.Generic;
using GameBrains.Actions;
using GameBrains.Actuators;
using GameBrains.Memories;
using GameBrains.Minds;
using GameBrains.Percepts;
using GameBrains.PerformanceMeasures;
using GameBrains.Sensors;
using UnityEngine;

namespace GameBrains.Entities.Agents
{
    // Control how agents is moved
    public enum MotorTypes
    {
        None,
        TransformLerp,
        //TransformTranslate,
        CharacterController,
        Rigidbody
    };

    // Control how we pick where to move
    public enum TargetTypes
    {
        None,
        First,
        Closest,
        Valued,
        Random
    }

    // Control how we handle the current action
    public enum ThinkTypes
    {
        Replace,
        Merge
    }
    
    public class Agent : Entity
    {
        [SerializeField] MotorTypes motorType;
        public MotorTypes MotorType => motorType;
        
        [SerializeField] TargetTypes targetType;
        public TargetTypes TargetType => targetType;

        [SerializeField] ThinkTypes thinkTypes;
        public ThinkTypes ThinkType => thinkTypes;

        [SerializeField] protected PerformanceMeasure performanceMeasure;
        public virtual PerformanceMeasure PerformanceMeasure
        {
            get => performanceMeasure;
            set => performanceMeasure = value;
        }

        [SerializeField] protected List<Sensor> sensors = new List<Sensor>();
        
        public virtual List<Sensor> Sensors
        {
            get => sensors;
            set => sensors = value;
        }

        [SerializeField] protected Mind mind;
        public virtual Mind Mind
        {
            get => mind;
            set => mind = value;
        }

        [SerializeField] protected List<Actuator> actuators = new List<Actuator>();

        public virtual List<Actuator> Actuators
        {
            get => actuators;
            set => actuators = value;
        }

        [SerializeField] protected Memory memory;

        public virtual Memory Memory
        {
            get => memory;
            set => memory = value;
        }

        // TODO: Modify to continue or interrupt action currently in progress
        protected List<Action> currentActions = new List<Action>();
        protected List<Percept> currentPercepts = new List<Percept>();

        public override void Awake()
        {
            base.Awake();
            SetUpMotorComponents();
            
        }

        public override void Update()
        {
            base.Update ();
            Sense();
            Think();
            Act();
        }

    /**********************************************************************************************/
    /* Handle things the agent does */ 

        /*Go through all sensors and get back result from all sensors */
        protected void Sense(){
            currentPercepts = new List<Percept>();

            foreach (Sensor sensor in Sensors)
            {
                if (sensor.SensorUpdateRegulator.IsReady)
                {
                    var currentPercept = sensor.Sense();
                    if (currentPercept != null)
                    {
                        currentPercepts.Add(sensor.Sense());
                    }
                }
            }
            /*if (Memory != null)
            {
                Memory.Record(currentPercepts);
            }*/
        }

        protected void Think(){
            if (Mind != null && Mind.MindUpdateRegulator.IsReady)
            {
                // TODO: Should we deal with in-progress actions or just drop them
                // Actions can either be replaced or somehow merged - we can use memory to control adding 
                switch (ThinkType)
                {
                    case ThinkTypes.Replace:
                        currentActions = Mind.Think(currentPercepts);
                        break;
                    case ThinkTypes.Merge:
                        MergeActions(Mind.Think(currentPercepts));
                        break;
                    default:
                        Debug.LogWarning("Unsupported ThinkType");
                        break;
                }       
            }
            /*if (Memory != null)
            {
                Memory.Record(currentActions);
            }*/
        }
        protected void Act(){
            foreach (Actuator actuator in Actuators)
            {
                if (actuator.ActuatorUpdateRegulator.IsReady)
                {
                    actuator.Act(currentActions);
                    CheckStatus();
                }
            }
            /*if (Memory != null)
            {
                Memory.Record(currentActions);
            }*/
        }

        /* If the the think type is set to merge we will try to merge actions */
        protected virtual void MergeActions(List<Action> newActions)
        {
            foreach (Action action in newActions)
            {
                bool added = false;
                for (int i = 0; i < currentActions.Count; i++)
                {
                    // TODO: Can we have different actions of the same type??
                    if (currentActions[i].GetType() == action.GetType())
                    {
                        print("Action Interrupted: " + currentActions[i]);
                        currentActions[i] = action; // replace
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    currentActions.Add(action);
                }
            }
        }
    /**********************************************************************************************/
    /* Agent self-awareness */     

        protected void CheckStatus()
        {
            for (int i = 0; i < currentActions.Count; i++)
            {
                if (currentActions[i].completionStatus == CompletionsStates.Complete)
                {
                    print("Action Completed: " + currentActions[i]);
                    currentActions.RemoveAt(i);
                    i--;
                }
                else
                {
                    currentActions[i].timeToLive -= Time.deltaTime;
                    if (currentActions[i].timeToLive <= 0)
                    {
                        print("Action Timed Out: " + currentActions[i]);
                        currentActions[i].completionStatus = CompletionsStates.Failed;
                        // Let failed remove this action
                    }

                    if (currentActions[i].completionStatus == CompletionsStates.Failed)
                    {
                        print("Action Failed: " + currentActions[i]);
                        currentActions.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        
    /**********************************************************************************************/
    /* Handle setting up Unity components in code */

        /* Setup components needed to move the agent depending on the selected motor type */
        protected void SetUpMotorComponents(){
            switch (MotorType)
            {
                case MotorTypes.CharacterController:
                    SetupCharacterController();
                    break;
                case MotorTypes.Rigidbody:
                    SetupRigidbody();
                    break;
            }
        }

        protected void SetupCharacterController()
        {
            if (gameObject.GetComponent<CharacterController>() != null) return;
            var characterController = gameObject.AddComponent<CharacterController>();
            Vector3 center = characterController.center;
            center.y = 1; // Agent's pivot is at 0, not its center
            characterController.center = center;
        }

        // programmatically set up rigid body component 
        protected void SetupRigidbody()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.constraints = RigidbodyConstraints.FreezeRotationX |RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
                rb.mass=100;
                rb.drag = 1.5f;
            }
        
            CapsuleCollider collider = gameObject.GetComponent<CapsuleCollider>();
        
            if (collider == null)
            {
                collider = gameObject.AddComponent<CapsuleCollider>();
                Vector3 center = collider.center;
                center.y = 1; // Agent's pivot is at 0, not its center
                collider.center = center;
                collider.height = 2;
            }
        }
    }
}