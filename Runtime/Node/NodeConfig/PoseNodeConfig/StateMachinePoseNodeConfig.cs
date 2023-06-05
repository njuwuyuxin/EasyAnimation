using System;
using System.Collections.Generic;

namespace AnimationGraph
{
    [Serializable]
    public class StateMachinePoseNodeConfig : PoseNodeConfig
    {
        public int defaultStateId;
        //states order == port order
        public List<StatePoseNodeConfig> states;
        public List<TransitionConfig> transitions;
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StateMachineNode stateMachineNode = new StateMachineNode();
            stateMachineNode.m_NodeConfig = this;
            stateMachineNode.InitializeGraphNode(graphRuntime);
            return stateMachineNode;
        }
    }
}
