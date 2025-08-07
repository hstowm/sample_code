namespace ROI
{
    using System.Collections;
    using UnityEngine;
    public class FearStayAwayEffectData
    {
        public float timeRandomPosition = 2;
        public Vector3 positionCenterFear = Vector3.zero;
        public IEnumerator handle;
        public uint netID;
    }
}
