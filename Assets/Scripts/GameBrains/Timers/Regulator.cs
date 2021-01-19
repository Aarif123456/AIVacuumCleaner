using UnityEngine;

namespace GameBrains.Timers
{
    public enum RegulatorDistribution
    {
        Uniform,
        Binomial,
        PositiveBinomial,
        Curve,
        Gaussian
    };

    public enum RegulatorMode
    {
        NoRun,
        NoWait,
        Normal
    }
    
    public class Regulator
    {
        float nextUpdateTime;
        float minimumTimeMs;
        public RegulatorMode Mode { get; set; }

        public Regulator()
        {
            nextUpdateTime = Time.time;
            MinimumTimeMs = 0;
            MaximumDelayMs = 0;
            Mode = RegulatorMode.NoWait;
            DelayDistribution = RegulatorDistribution.PositiveBinomial;
            DistributionCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
        }

        public bool IsReady
        {
            get
            {
                if (Mode == RegulatorMode.NoRun || Time.time < nextUpdateTime) return false;

                if (Mode == RegulatorMode.NoWait) return true;

                float aRandom = GetRandom(DelayDistribution);
                
                nextUpdateTime = Time.time + (MinimumTimeMs + aRandom * MaximumDelayMs) / 1000f;

                return true;

            }
        }
        
        public RegulatorDistribution DelayDistribution { get; set; }

        public float MinimumTimeMs
        {
            get => minimumTimeMs;
            set => minimumTimeMs = value <= 0 ? 0 : value;
        }

        public float UpdatesPerSecond
        {
            get => MinimumTimeMs > 0 ? 1000f / MinimumTimeMs : MinimumTimeMs;

            set => MinimumTimeMs = value > 0 ? 1000f / value : 0;
        }

        public float MaximumDelayMs { get; set; }

        public AnimationCurve DistributionCurve { get; set; }

        float GetRandom(RegulatorDistribution regulatorDistribution)
        {
            float aRandom;

            switch (regulatorDistribution)
            {
                case RegulatorDistribution.Binomial:
                    aRandom = Random.value - Random.value;
                    break;
                case RegulatorDistribution.PositiveBinomial:
                {
                    aRandom = Random.value - Random.value;

                    if (aRandom < 0)
                    {
                        aRandom *= -1f;
                    }

                    break;
                }
                case RegulatorDistribution.Curve:
                    aRandom = DistributionCurve.Evaluate(Random.value);
                    break;
                case RegulatorDistribution.Gaussian:
                    aRandom = RandomGaussian();
                    break;
                // default to uniform distribution
                default:
                    aRandom = Random.value;
                    break;
            }
            
            return aRandom;
        }
        
        /// <summary>
        /// Generates a clamped normally distributed random value
        /// </summary>
        /// <param name="minValue">Minimum value (-3 sigma)</param>
        /// <param name="maxValue">Maximum value (3 sigma)</param>
        /// <returns></returns>
        float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
        {
            float u, v, S;

            do
            {
                u = 2.0f * Random.value - 1.0f;
                v = 2.0f * Random.value - 1.0f;
                S = u * u + v * v;
            } while (S >= 1.0f);

            // Standard Normal Distribution
            float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

            // Normal Distribution centered between the min and max value
            // and clamped following the "three-sigma rule"
            float mean = (minValue + maxValue) / 2.0f;
            float sigma = (maxValue - mean) / 3.0f;
            return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
        }
    }
}