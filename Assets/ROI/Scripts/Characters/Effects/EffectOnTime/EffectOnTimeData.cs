using System;

namespace ROI
{
    public readonly struct EffectOnTimeData<T>
    {
        public readonly float SecondPerApply;
        public readonly float Duration;
        public readonly ChampionData Champion;
        public readonly T Data;
        public readonly Action<ChampionData, T> ApplyHandle;

        public EffectOnTimeData(ChampionData championData, T data, Action<ChampionData, T> applyHandle, float duration,
            float secondPerApply = 1)
        {
            Champion = championData;
            Data = data;
            ApplyHandle = applyHandle;
            Duration = duration;
            SecondPerApply = secondPerApply;
        }
    }
}