using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{

    public abstract class INode
    {
        public int id;
        
        public virtual ENodeType nodeType { get; }

        public abstract void InitializeGraphNode(AnimationGraphRuntime animationGraphRuntime);

        public virtual void OnStart() { }

        public virtual void OnUpdate(float deltaTime) { }

        public virtual void AddInputNode(INode inputNode, int slotIndex)
        {
            throw new NotImplementedException();
        }
        
        public virtual void AddOutputNode(INode outputNode)
        {
            throw new NotImplementedException();
        }
    }
}
