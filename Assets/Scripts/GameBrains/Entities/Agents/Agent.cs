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
    public enum MotorTypes
    {
        None,
        TransformLerp,
        //TransformTranslate,
        CharacterController,
        Rigidbody
    };

    public enum TargetTypes
    {
        None,
        First,
        Closest,
        Valued,
        Random
    }

    public enum ThinkTypes
    {
        Replace,
        Add,
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
        // TODO: Make protected and add public accessors
        public virtual PerformanceMeasure PerformanceMeasure
        {
            get => performanceMeasure;
            set => performanceMeasure = value;
        }

        [SerializeField] protected List<Sensor> sensors = new List<Sensor>();
        
        // TODO: Make protected and add public accessors
        public virtual List<Sensor> Sensors
        {
            get => sensors;
            set => sensors = value;
        }

        [SerializeField] protected Mind mind;
        // TODO: Make protected and add public accessors
        public virtual Mind Mind
        {
            get => mind;
            set => mind = value;
        }

        [SerializeField] protected List<Actuator> actuators = new List<Actuator>();
        // TODO: Make protected and add public accessors
        public virtual List<Actuator> Actuators
        {
            get => actuators;
            set => actuators = value;
        }

        [SerializeField] protected Memory memory;

        // TODO: Make protected and add public accessors
        public virtual Memory Memory
        {
            get => memory;
            set => memory = value;
        }

        // TODO: Modify to continue or interrupt action currently in progress
        protected List<Action> currentActions = new List<Action>();

        public override void Awake()
        {
            base.Awake();

            switch (MotorType)
            {
                case MotorTypes.CharacterController:
                    SetupCharacterController();
                    break;
                // case MotorTypes.Rigidbody:
                //     SetupRigidbody();
                //     break;
            }
        }

        public override void Update()
        {
            base.Update ();

            List<Percept> currentPercepts = new List<Percept>();

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

            // if (Memory != null)
            // {
            //     Memory.Record(currentPercepts);
            // }

            if (Mind != null && Mind.MindUpdateRegulator.IsReady)
            {
                // TODO: Should we deal with inprogress actions or just drop them
                ChooseThinkType(currentPercepts);

                print("Action count = " + currentActions.Count);
            }
            
            // if (Memory != null)
            // {
            //     Memory.Record(currentActions);
            // }

            foreach (Actuator actuator in Actuators)
            {
                if (actuator.ActuatorUpdateRegulator.IsReady)
                {
                    actuator.Act(currentActions);
                    //print("Action count = " + currentActions.Count);

                    CheckStatus();
                }
            }

            // if (Memory != null)
            // {
            //     Memory.Record(currentActions);
            // }
        }

        protected void ChooseThinkType(List<Percept> currentPercepts)
        {
            if (ThinkType == ThinkTypes.Replace)
            {
                currentActions = Mind.Think(currentPercepts);
            }
            else if (ThinkType == ThinkTypes.Add)
            {
                currentActions.AddRange(Mind.Think(currentPercepts));
            }
            else if (ThinkType == ThinkTypes.Merge)
            {
                MergeActions(Mind.Think(currentPercepts));
            }
            else
            {
                Debug.LogWarning("Unsupported ThinkType");
            }
        }

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

        protected void MergeActions(List<Action> newActions)
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

        protected void SetupCharacterController()
        {
            if (gameObject.GetComponent<CharacterController>() != null) return;

            var characterController = gameObject.AddComponent<CharacterController>();
            Vector3 center = characterController.center;
            center.y = 1; // Agent's pivot is at 0, not its center
            characterController.center = center;
        }

        protected void SetupRigidbody()
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                //rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
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