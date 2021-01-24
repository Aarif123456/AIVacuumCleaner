using GameBrains.Timers;
using UnityEngine;

namespace GameBrains.TestScripts
{
    public class RegulatorTester : MonoBehaviour
    {
        public float minimumTimeMs;
        public float maximumDelayMs;
        public RegulatorMode mode;
        public RegulatorDistribution regulatorDistribution;
        public AnimationCurve distributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        float timeSinceLastReady = 0;

        Regulator regulator;

        void Start()
        {
            regulator = new Regulator
            {
                MinimumTimeMs = minimumTimeMs,
                MaximumDelayMs = maximumDelayMs,
                Mode = mode,
                DelayDistribution = regulatorDistribution,
                DistributionCurve = distributionCurve
            };
        }

        void Update()
        {
            timeSinceLastReady += Time.deltaTime;
            
            if (regulator.IsReady)
            {
                print("Ready after: " + timeSinceLastReady.ToString("N1") + " seconds.");
                timeSinceLastReady = 0;
            }
        }
    }
}