using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StringSelectorNode : GraphNode
    {
        public override ENodeType nodeType => ENodeType.StringSelectorNode;
        protected override bool m_DrawInspectorCustomize => true;

        private List<string> m_Selections = new List<string>();

        public StringSelectorNode(AnimationGraphView graphView, Vector2 position) : base(graphView,position)
        {
            nodeName = "StringSelector";
            ColorUtility.TryParseHtmlString("#663366", out var titleColor);
            titleContainer.style.backgroundColor = new StyleColor(titleColor);
        }

        public override void InitializeDefault()
        {
            base.InitializeDefault();
            m_NodeConfig = new StringSelectorPoseNodeConfig();
            m_NodeConfig.SetId(id);
            CreatePort(Direction.Output, Port.Capacity.Multi, "Output", NodePort.EPortType.PosePort, 0);
            CreatePort(Direction.Input, Port.Capacity.Multi, "Condition", NodePort.EPortType.ValuePort, 0);
        }

        public override void OnSave()
        {
            var config = m_NodeConfig as StringSelectorPoseNodeConfig;
            config.selections.Clear();
            config.selections.AddRange(m_Selections);
        }

        public override void LoadNodeData(NodeData data)
        {
            base.LoadNodeData(data);
            var config = m_NodeConfig as StringSelectorPoseNodeConfig;
            m_Selections.Clear();
            m_Selections.AddRange(config.selections);
        }
        
        public override void OnNodeInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                m_Selections.Add("Default String");
                CreatePort(Direction.Input, Port.Capacity.Multi, m_Selections[m_Selections.Count -1], NodePort.EPortType.PosePort, m_Selections.Count -1);
            }
            
            if (GUILayout.Button("-"))
            {
                if (m_Selections.Count > 0)
                {
                    var selectionPort = GetInputPort(NodePort.EPortType.PosePort, m_Selections.Count - 1);
                    DeletePort(selectionPort);
                    m_Selections.RemoveAt(m_Selections.Count - 1);
                }
            }
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < m_Selections.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(20));
                m_Selections[i] = EditorGUILayout.TextField(m_Selections[i]);
                var selectionPort = GetInputPort(NodePort.EPortType.PosePort, i);
                selectionPort.portName = m_Selections[i];
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
