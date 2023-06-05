using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AnimationGraph
{
    public abstract class ValueNode<TNodeConfig> : IValueNodeInterface where TNodeConfig : ValueNodeConfig
    {
        internal TNodeConfig m_NodeConfig;
    }
}
