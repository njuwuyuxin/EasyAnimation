using UnityEngine;

namespace AnimationGraph
{
    public class EasyAnimator : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public EAnimationMode animationMode = EAnimationMode.AnimationClip;
        public AnimationClip animationClip;
        
        private AnimationCore m_AnimationCore;

        // Start is called before the first frame update
        void Start()
        {
            m_AnimationCore = new AnimationCore();
            m_AnimationCore.Initialize(skinnedMeshRenderer);
            m_AnimationCore.animationMode = animationMode;
            m_AnimationCore.SetTestAnimationClip(animationClip);
        }

        // Update is called once per frame
        void Update()
        {
            m_AnimationCore.Update(Time.deltaTime);
        }
    }
}