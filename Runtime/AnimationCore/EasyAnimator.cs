using UnityEngine;

namespace AnimationGraph
{
    public class EasyAnimator : MonoBehaviour
    {
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public EAnimationMode animationMode = EAnimationMode.AnimationClip;
        public AnimationClip animationClip;
        
        //Only support RootMotion XZ for now
        public bool enableRootMotion = false;
        
        private AnimationCore m_AnimationCore;

        // Start is called before the first frame update
        void Start()
        {
            m_AnimationCore = new AnimationCore();
            m_AnimationCore.Initialize(skinnedMeshRenderer);
            m_AnimationCore.animationMode = animationMode;
            m_AnimationCore.enableRootMotion = enableRootMotion;
            m_AnimationCore.SetTestAnimationClip(animationClip);
        }

        // Update is called once per frame
        void Update()
        {
            m_AnimationCore.Update(Time.deltaTime);
            if (enableRootMotion)
            {
                ProcessRootMotion();
            }
        }

        private void ProcessRootMotion()
        {
            Vector3 deltaPosition = new Vector3(m_AnimationCore.rootMotionData.deltaPosition.x, 0,
                m_AnimationCore.rootMotionData.deltaPosition.z);
            transform.Translate(deltaPosition);
        }
    }
}