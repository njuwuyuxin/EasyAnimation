
using UnityEngine.Animations;

namespace AnimationGraph
{
    public abstract class PoseNode<TNodeConfig> : IPoseNodeInterface where TNodeConfig : PoseNodeConfig
    {
        internal TNodeConfig m_NodeConfig;
    }
}
