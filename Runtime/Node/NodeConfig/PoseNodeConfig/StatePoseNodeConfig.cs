using System;
using UnityEngine;

namespace AnimationGraph
{
    [Serializable]
    public class StatePoseNodeConfig : PoseNodeConfig
    {
        public string stateName;
        public Vector2 position;
        
        public override INode GenerateNode(AnimationGraphRuntime graphRuntime)
        {
            StateNode stateNode = new StateNode();
            stateNode.m_NodeConfig = this;
            stateNode.InitializeGraphNode(graphRuntime);
            return stateNode;
        }
    }
}
