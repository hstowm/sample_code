namespace ROI
{
    public interface IEffectOnTime
    {
        bool OnNextUpdate();
        void RefreshTime();
        bool IsValid { get; }
        void Cancel();
        float RemainingSecond { get; }
        public float Duration { get; }
        public float SecondPerApply { get; }
        public float RunningTime { get; }
    }
}