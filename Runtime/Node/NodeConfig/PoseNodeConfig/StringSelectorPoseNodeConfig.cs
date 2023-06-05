using System;
using System.Collections.Generic;

namespace AnimationGraph
{
    [Serializable]
    public class StringSelectorPoseNodeConfig : PoseNodeConfig
    {
        public float blendTime = 0.25f;
        // index 代表 selection port index
        public List<string> selections = new List<string>();
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StringSelectorNode stringSelectorNode = new StringSelectorNode();
            stringSelectorNode.m_NodeConfig = this;
            stringSelectorNode.InitializeGraphNode(graphRuntime);
            return stringSelectorNode;
        }
    }
}