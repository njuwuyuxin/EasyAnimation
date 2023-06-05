using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView : GraphViewBase
    {
        private const string k_StyleSheetPrefix = "StyleSheet/";
        private VisualElement m_Container;
        private AnimationGraphAsset m_AnimationGraphAsset;
        internal AnimationGraphInspector inspector => m_Inspector;
        public ParameterBoard parameterBoard => m_ParameterBoard;
        private ParameterBoard m_ParameterBoard;
        private AnimationGraphInspector m_Inspector;
        private StateMachineGraphView m_StateMachineGraphView;

        internal AnimationGraphView(VisualElement container, ParameterBoard parameterBoard, AnimationGraphInspector inspector)
        {
            m_Container = container;
            m_ParameterBoard = parameterBoard;
            m_Inspector = inspector;
            AddDefaultManipulators();
            this.AddManipulator(CreateContextualMenu());
            AddStyleSheet();
            RegisterCallbacks();
        }

        public void OpenStateMachineGraphView(StateMachineNode stateMachineNode)
        {
            m_Container.Remove(this);
            
            m_StateMachineGraphView = new StateMachineGraphView(m_Container, this, stateMachineNode, m_ParameterBoard, m_Inspector);
            m_Container.Add(m_StateMachineGraphView);
            m_StateMachineGraphView.StretchToParentSize();
        }

        public void CloseStateMachineGraphView()
        {
            if (m_StateMachineGraphView != null)
            {
                m_Container.Remove(m_StateMachineGraphView);
                m_Container.Add(this);
                this.StretchToParentSize();
                m_StateMachineGraphView = null;
            }
        }

        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    menuEvent.menu.MenuItems().Clear();
                    menuEvent.menu.AppendAction(
                        "Add FinalPose Node",
                        actionEvent => CreateDefaultNode(ENodeType.FinalPoseNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add AnimationClip Node",
                        actionEvent => CreateDefaultNode(ENodeType.AnimationClipNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add BoolSelector Node",
                        actionEvent => CreateDefaultNode(ENodeType.BoolSelectorNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add StringSelector Node",
                        actionEvent => CreateDefaultNode(ENodeType.StringSelectorNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add Blend1D Node",
                        actionEvent => CreateDefaultNode(ENodeType.Blend1DNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                    menuEvent.menu.AppendAction(
                        "Add StateMachine Node",
                        actionEvent => CreateDefaultNode(ENodeType.StateMachineNode, MouseToViewPosition(actionEvent.eventInfo.mousePosition))
                    );
                });
            return contextualMenuManipulator;
        }

        private void AddStyleSheet()
        {
            var graphViewStyleSheet = Resources.Load<StyleSheet>(k_StyleSheetPrefix + "AnimationGraphView");
            if (graphViewStyleSheet != null)
            {
                styleSheets.Add(graphViewStyleSheet);
            }
        }
    }
}
