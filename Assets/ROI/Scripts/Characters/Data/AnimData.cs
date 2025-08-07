using System;

namespace ROI
{
    /// <summary>
    /// Auto Attack Animation Data
    /// </summary>
    [Serializable]
    public class AnimData
    {
#if UNITY_EDITOR
        /// <summary>
        /// Animation Name
        /// </summary>
        public string animName = "attack";
#endif

        /// <summary>
        /// Total Frame of Animation Clip
        /// </summary>
        public float animFrames = 35f;

        /// <summary>
        /// Normal Speed Animation Clip. Frame Per Second
        /// </summary>
        public float animSpeed = 30;

        /// <summary>
        /// Total Number Meter Per Speed Animation
        /// </summary>
       // public float meterPerSpeedAnim = 0.0268f;// * 35f;

        public float totalMeterPerAnim = 0.938f;// 0.0268f * 35f;
    }
}