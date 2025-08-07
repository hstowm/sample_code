using UnityEngine;

namespace ROI
{
    class EffectOnTime<T> : IEffectOnTime
    {
        public bool IsValid { get; private set; }

        private float _tickTime;
        private float _remainingSecond;
        public float RemainingSecond => _remainingSecond;

        public float Duration => _effectOnTimeData.Duration;
        public float SecondPerApply => _effectOnTimeData.SecondPerApply;

        public float RunningTime => _tickTime;

        public void RefreshTime()
        {
            if (IsValid == false)
            {
                Logs.Error("Cant refresh time an effect is out of date");
                return;
            }

            _remainingSecond = _effectOnTimeData.Duration;
            _tickTime = 0;
        }

        public bool OnNextUpdate()
        {
            if (IsValid == false)
                return false;

            if (_remainingSecond == 0)
            {
                IsValid = false;
                return false;
            }

            _tickTime += Time.deltaTime;

            if (_tickTime >= _effectOnTimeData.SecondPerApply)
            {
                OnTick();
            }

            return true;
        }

        private void OnTick()
        {
            _effectOnTimeData.ApplyHandle.Invoke(_effectOnTimeData.Champion, _effectOnTimeData.Data);
            _tickTime -= _effectOnTimeData.SecondPerApply;
            _remainingSecond -= _effectOnTimeData.SecondPerApply;

            IsValid = _remainingSecond >= _effectOnTimeData.SecondPerApply;
        }

        public void Cancel()
        {
            _remainingSecond = 0;
            _tickTime = 0;
            IsValid = false;
        }

        public EffectOnTime(EffectOnTimeData<T> effectOnTimeData)
        {
            _effectOnTimeData = effectOnTimeData;
            _remainingSecond = _effectOnTimeData.Duration;
            _tickTime = 0;
            IsValid = _remainingSecond >= _effectOnTimeData.SecondPerApply;
        }

        private readonly EffectOnTimeData<T> _effectOnTimeData;
    }
}