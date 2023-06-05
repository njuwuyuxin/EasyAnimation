using UnityEditor;
using UnityEngine;

namespace AnimationGraph.Editor
{
    public class IMGUIInspector : UnityEditor.Editor
    {
        class NodeInspectorObject : ScriptableObject
        {
            [SerializeReference]
            public NodeConfig m_NodeConfig;
        }

        class EdgeInspectorObject : ScriptableObject
        {
            [SerializeReference]
            public EdgeConfig m_EdgeConfig;
        }

        private enum EContentType
        {
            Null,
            Node,
            Edge,
        }

        private EContentType m_ContentType = EContentType.Null;

        private GraphNode m_GraphNode;
        private NodeConfig m_NodeConfig => m_GraphNode.nodeConfig;
        private StateTransition m_StateTransition;

        private EdgeConfig m_EdgeConfig => m_StateTransition.edgeConfig;
        
        private NodeInspectorObject m_NodeInspectorObject;
        private EdgeInspectorObject m_EdgeInspectorObject;
        private SerializedObject m_SerializedObject;
        private bool m_DrawInspectorCustomize;

        public override void OnInspectorGUI()
        {

            if (m_ContentType == EContentType.Null)
            {
                return;
            }
            else if (m_ContentType == EContentType.Node)
            {
                if (!m_DrawInspectorCustomize)
                {
                    if (m_SerializedObject != null)
                    {
                        var nodeConfig = m_SerializedObject.FindProperty("m_NodeConfig");
                        if (nodeConfig != null)
                        {
                            while (nodeConfig.NextVisible(true))
                            {
                                EditorGUILayout.PropertyField(nodeConfig);
                            }

                            if (m_SerializedObject.hasModifiedProperties)
                            {
                                m_SerializedObject.ApplyModifiedProperties();
                                m_GraphNode.OnNodeConfigUpdate();
                            }
                        }
                    }
                }

                if (m_GraphNode != null)
                {
                    m_GraphNode.OnNodeInspectorGUI();
                }
            }
            else if (m_ContentType == EContentType.Edge)
            {
                if (!m_DrawInspectorCustomize)
                {
                    if (m_SerializedObject != null)
                    {
                        var edgeConfig = m_SerializedObject.FindProperty("m_EdgeConfig");
                        if (edgeConfig != null)
                        {
                            while (edgeConfig.NextVisible(true))
                            {
                                EditorGUILayout.PropertyField(edgeConfig);
                            }

                            if (m_SerializedObject.hasModifiedProperties)
                            {
                                m_SerializedObject.ApplyModifiedProperties();
                                m_StateTransition.OnEdgeConfigUpdate();
                            }
                        }
                    }
                }

                if (m_StateTransition != null)
                {
                    m_StateTransition.OnEdgeInspectorGUI(m_SerializedObject);
                }
            }
        }

        public void SetGraphNode(GraphNode graphNode, bool drawInspectorCustomize)
        {
            ClearInspector();
            m_ContentType = EContentType.Node;
            m_DrawInspectorCustomize = drawInspectorCustomize;
            m_GraphNode = graphNode;
            m_NodeInspectorObject = ScriptableObject.CreateInstance<NodeInspectorObject>();
            m_NodeInspectorObject.m_NodeConfig = m_NodeConfig;
            m_SerializedObject = new SerializedObject(m_NodeInspectorObject);
        }

        public void SetEdge(StateTransition edge, bool drawInspectorCustomize)
        {
            ClearInspector();
            m_ContentType = EContentType.Edge;
            m_DrawInspectorCustomize = drawInspectorCustomize;
            m_StateTransition = edge;
            m_EdgeInspectorObject = ScriptableObject.CreateInstance<EdgeInspectorObject>();
            m_EdgeInspectorObject.m_EdgeConfig = m_EdgeConfig;
            m_SerializedObject = new SerializedObject(m_EdgeInspectorObject);
        }

        public void ClearInspector()
        {
            if (m_NodeInspectorObject != null)
            {
                DestroyImmediate(m_NodeInspectorObject);
            }

            if (m_EdgeInspectorObject != null)
            {
                DestroyImmediate(m_EdgeInspectorObject);
            }

            m_DrawInspectorCustomize = false;
            m_GraphNode = null;

            m_NodeInspectorObject = null;
            m_SerializedObject = null;
            m_ContentType = EContentType.Null;
        }
    }
}
