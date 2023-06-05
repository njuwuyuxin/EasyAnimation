using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    internal class AnimationGraphInspector : VisualElement
    {
        private enum EInspectorType
        {
            IMGUI,
            UIElement
        }
        
        public IMGUIInspector IMGUIInspector { get; private set; }
        private IMGUIContainer m_IMGUIContainer;
        private VisualElement m_UIElementContainer;

        private EInspectorType m_InspectorType = EInspectorType.IMGUI;

        public AnimationGraphInspector()
        {
            CreateTitleGUI();

            m_IMGUIContainer = new IMGUIContainer();
            m_UIElementContainer = new VisualElement();
            Add(m_IMGUIContainer);
            
            IMGUIInspector = ScriptableObject.CreateInstance<IMGUIInspector>();
            m_IMGUIContainer.onGUIHandler = IMGUIInspector.OnInspectorGUI;
        }

        private void CreateTitleGUI()
        {
            VisualElement titleContainer = new VisualElement();
            Label title = new Label("Inspector");
            title.style.fontSize = 14;
            title.style.color = Color.white;
            title.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>(FontStyle.Bold);
            titleContainer.Add(title);
            Add(titleContainer);
        }
        
        public void SetCustomContent(VisualElement content)
        {
            if (m_InspectorType == EInspectorType.IMGUI)
            {
                Remove(m_IMGUIContainer);
                Add(m_UIElementContainer);
                m_InspectorType = EInspectorType.UIElement;
            }
            
            m_UIElementContainer.Clear();
            m_UIElementContainer.Add(content);
        }

        public void SetGraphNodeIMGUI(GraphNode graphNode, bool drawInspectorCustomize)
        {
            if (m_InspectorType == EInspectorType.UIElement)
            {
                Remove(m_UIElementContainer);
                Add(m_IMGUIContainer);
                m_InspectorType = EInspectorType.IMGUI;
            }

            IMGUIInspector.SetGraphNode(graphNode, drawInspectorCustomize);
        }

        public void SetEdgeIMGUI(StateTransition edge, bool drawInspectorCustomize)
        {
            if (m_InspectorType == EInspectorType.UIElement)
            {
                Remove(m_UIElementContainer);
                Add(m_IMGUIContainer);
                m_InspectorType = EInspectorType.IMGUI;
            }

            IMGUIInspector.SetEdge(edge, drawInspectorCustomize);
        }

        public void ClearInspector()
        {
            if (m_InspectorType == EInspectorType.IMGUI)
            {
                IMGUIInspector.ClearInspector();
            }

            if (m_InspectorType == EInspectorType.UIElement)
            {
                m_UIElementContainer.Clear();
            }
        }
    }
}