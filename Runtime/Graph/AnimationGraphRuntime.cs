using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace AnimationGraph
{
    public class AnimationGraphRuntime
    {
        private AnimationActor m_Actor;
        private CompiledAnimationGraph m_CompiledAnimationGraph;
        public PlayableGraph m_PlayableGraph;
        private AnimationPlayableOutput m_Output;
        public Playable m_FinalPosePlayable;
        public AnimationScriptPlayable m_PostProcessPlayable;
        private FinalPoseNode m_FinalPoseNode;
        private Dictionary<int, INode> m_Id2NodeMap;
        private Dictionary<int, GraphParameter> m_Id2ParameterMap;
        private Dictionary<string, GraphParameter> m_String2ParameterMap;
        
        public AnimationGraphRuntime(AnimationActor actor, CompiledAnimationGraph compiledAnimationGraph)
        {
            m_Actor = actor;
            m_CompiledAnimationGraph = compiledAnimationGraph;
            m_PlayableGraph = PlayableGraph.Create();
            m_PlayableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            m_Output = AnimationPlayableOutput.Create(m_PlayableGraph,
                m_Actor.gameObject.name + "_" + m_Actor.gameObject.GetInstanceID(), m_Actor.animator);
            GenerateAnimationGraph();
            m_FinalPosePlayable = Playable.Create(m_PlayableGraph, 1);
            PostProcessJob postProcessJob = new PostProcessJob();
            m_PostProcessPlayable = AnimationScriptPlayable.Create(m_PlayableGraph, postProcessJob, 1);
            m_PostProcessPlayable.ConnectInput(0, m_FinalPosePlayable, 0, 1f);
            
            m_Output.SetSourcePlayable(m_PostProcessPlayable);
        }

        public void Run()
        {
            m_FinalPoseNode.OnStart();
            m_PlayableGraph.Play();
        }

        private void GenerateAnimationGraph()
        {
            GenerateParameterMap();
            
            m_Id2NodeMap = new Dictionary<int, INode>();
            m_FinalPoseNode = (FinalPoseNode)m_CompiledAnimationGraph.finalPosePoseNode.GenerateNode(this);
            m_Id2NodeMap.Add(m_CompiledAnimationGraph.finalPosePoseNode.id, m_FinalPoseNode);
            foreach (var nodeConfig in m_CompiledAnimationGraph.nodes)
            {
                if (m_Id2NodeMap.ContainsKey(nodeConfig.id))
                {
                    continue;
                }
                var animationGraphNode = nodeConfig.GenerateNode(this);
                m_Id2NodeMap.Add(nodeConfig.id, animationGraphNode);
            }

            foreach (var connection in m_CompiledAnimationGraph.nodeConnections)
            {
                var sourceNode = m_Id2NodeMap[connection.sourceNodeId];
                var targetNode = m_Id2NodeMap[connection.targetNodeId];
                sourceNode.AddOutputNode(targetNode);
                targetNode.AddInputNode(sourceNode, connection.targetPortIndex);
            }
        }

        private void GenerateParameterMap()
        {
            m_Id2ParameterMap = new Dictionary<int, GraphParameter>();
            m_String2ParameterMap = new Dictionary<string, GraphParameter>();
            List<GraphParameter> runtimeParameter = new List<GraphParameter>();
            runtimeParameter.AddRange(m_CompiledAnimationGraph.parameters);
            foreach (var parameter in runtimeParameter)
            {
                m_Id2ParameterMap.Add(parameter.id, parameter);
                m_String2ParameterMap.Add(parameter.name, parameter);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            m_FinalPoseNode.OnUpdate(deltaTime);
        }

        public void Destroy()
        {
            m_PlayableGraph.Destroy();
        }

        public void SetBoolParameter(string parameterName, bool value)
        {
            if (m_String2ParameterMap.TryGetValue(parameterName, out var graphParameter))
            {
                if (graphParameter is BoolParameter boolParameter)
                {
                    boolParameter.value.boolValue = value;
                }
                foreach (var nodeId in  graphParameter.associatedNodes)
                {
                    if (m_Id2NodeMap.TryGetValue(nodeId, out var node))
                    {
                        BoolValueNode boolValueNode = (BoolValueNode) node;
                        boolValueNode.boolValue = value;
                    }
                }
            }
            else
            {
                Debug.LogError("Paramter \"" + parameterName + "\" not exist!");
            }
        }
        
        public void SetFloatParameter(string parameterName, float value)
        {
            if (m_String2ParameterMap.TryGetValue(parameterName, out var graphParameter))
            {
                if (graphParameter is FloatParameter floatParameter)
                {
                    floatParameter.value.floatValue = value;
                }
                foreach (var nodeId in  graphParameter.associatedNodes)
                {
                    if (m_Id2NodeMap.TryGetValue(nodeId, out var node))
                    {
                        FloatValueNode boolValueNode = (FloatValueNode) node;
                        boolValueNode.floatValue = value;
                    }
                }
            }
            else
            {
                Debug.LogError("Paramter \"" + parameterName + "\" not exist!");
            }
        }

        public void SetStringParameter(string parameterName, string value)
        {
            if (m_String2ParameterMap.TryGetValue(parameterName, out var graphParameter))
            {
                if (graphParameter is StringParameter stringParameter)
                {
                    stringParameter.value.stringValue = value;
                }
                foreach (var nodeId in  graphParameter.associatedNodes)
                {
                    if (m_Id2NodeMap.TryGetValue(nodeId, out var node))
                    {
                        StringValueNode stringValueNode = (StringValueNode) node;
                        stringValueNode.stringValue = value;
                    }
                }
            }
            else
            {
                Debug.LogError("Paramter \"" + parameterName + "\" not exist!");
            }
        }

        public GraphParameter GetParameterById(int id)
        {
            if (m_Id2ParameterMap.TryGetValue(id, out var parameter))
            {
                return parameter;
            }

            return null;
        }
    }
}
