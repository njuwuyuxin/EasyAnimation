using UnityEngine.Animations;

namespace AnimationGraph
{
    public struct PostProcessJob : IAnimationJob
    {
        public void ProcessAnimation(AnimationStream stream)
        {
            
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
            var position = stream.rootMotionPosition;
        }
    }
}
