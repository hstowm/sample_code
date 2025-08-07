using UnityEngine;

namespace ROI
{
    public interface IController
    {
        void Pause(IPauseHandle handle);

        void SetTrigger(int animHashID);

        void SetDeath();

        void StartAlive();

        void SetOutline(bool active);
        void SetOutlineColor(Color color);

        void ResetHexPosition();
    }
}