using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace AnimationGraph.Editor
{
    [Serializable]
    [CreateAssetMenu(fileName = "AnimationGraphAsset", menuName = "ScriptableObjects/AnimationGraphAsset")]
    public class AnimationGraphAsset : ScriptableObject
    {
        public List<NodeData> nodes;
        public List<PortData> ports;
        public List<EdgeData> edges;
        [SerializeReference]
        public List<ParameterData> parameters;
        
        
#if UNITY_EDITOR
        [OnOpenAsset]
        private static bool OnOpenAsset(int instanceID, int line)
        {
            var animationGraphAsset = EditorUtility.InstanceIDToObject(instanceID) as AnimationGraphAsset;
            if (animationGraphAsset == null)
            {
                return false;
            }

            AnimationGraphEditorWindow.ShowWindow(animationGraphAsset);
            return true;
        }
#endif
    }

    public enum ENodeType
    {
        BaseNode = 0,
        FinalPoseNode = 1,
        AnimationClipNode = 2,
        BoolSelectorNode = 3,
        StringSelectorNode = 4,
        Blend1DNode = 5,
        Blend2DNode = 6,
        StateMachineNode = 7,
        BoolValueNode = 101,
        IntValueNode = 102,
        FloatValueNode = 103,
        StringValueNode = 104,
        StateNode = 201,
    }

    public enum EPortDirection
    {
        Input = 0,
        Output = 1,
    }

    public enum EPortCapacity
    {
        Single = 0,
        Multi =1,
    }

    [Serializable]
    public class CustomSerializableData
    {
        
    }
    
    [Serializable]
    public class NodeData
    {
        public int id;
        public ENodeType nodeType;
        public float positionX;
        public float positionY;
        [SerializeReference]
        public NodeConfig nodeConfig;
        [SerializeReference]
        public CustomSerializableData customData;
    }

    [Serializable]
    public class PortData
    {
        public string portName;
        public int portId;
        public int nodeId;
        public NodePort.EPortType portType;
        public int portIndex;
        public EPortDirection direction;
        public EPortCapacity capacity;
    }

    [Serializable]
    public class EdgeData
    {
        public int inputPort;
        public int outputPort;
    }

    [Serializable]
    public class ParameterData
    {
        public int id;
        public string name;
        public List<int> associateNodes;
    }

    [Serializable]
    public class BoolParameterData : ParameterData
    {
        public bool value;
    }
    
    [Serializable]
    public class IntParameterData : ParameterData
    {
        public int value;
    }
    
    [Serializable]
    public class FloatParameterData : ParameterData
    {
        public float value;
    }
    
    [Serializable]
    public class StringParameterData : ParameterData
    {
        public string value;
    }
}