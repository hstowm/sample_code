using UnityEngine;

namespace ROI
{
    class ChampionAnimInitializer : IOnInitialized
    {
        private ChampionData _championData;
        public void OnInitialized(ChampionData championData)
        {
            _championData = championData;

            InitAnimData();
        }
        
        /// <summary>
        /// Init Animation Data
        /// </summary>
        private void InitAnimData()
        {
            var ctrl = _championData.animatorNetwork.animator.runtimeAnimatorController;

            foreach (var clip in ctrl.animationClips)
            {
                // setup attack data
                if (Animator.StringToHash(clip.name) == AnimHashIDs.Attack)
                {
                    SetupAttackAnim(clip);
                    continue;
                }

                // setup move data
                if (Animator.StringToHash(clip.name) == AnimHashIDs.Run)
                {
                    SetupMoveAnim(clip);
                }
            }
        }

        /// <summary>
        /// Setup Attack Animation Data
        /// </summary>
        /// <param name="clip"></param>
        private void SetupAttackAnim(AnimationClip clip)
        {
            _championData.attackAnim.animSpeed = clip.frameRate;
            _championData.attackAnim.animFrames = clip.frameRate * clip.length;
#if UNITY_EDITOR
            _championData.attackAnim.animName = clip.name;
#endif
            _championData.attackAnim.totalMeterPerAnim = 0;
        }

        /// <summary>
        /// Setup Movement animation data
        /// </summary>
        /// <param name="clip"></param>
        private void SetupMoveAnim(AnimationClip clip)
        {
            _championData.moveAnim.animSpeed = clip.frameRate;
            _championData.moveAnim.animFrames = clip.frameRate * clip.length;
#if UNITY_EDITOR
            _championData.moveAnim.animName = clip.name;
#endif
        }
    }
}