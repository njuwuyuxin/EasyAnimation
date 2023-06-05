using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class StateMachineGraphView : GraphViewBase
    {
        private const string k_StyleSheetPrefix = "StyleSheet/";
        internal AnimationGraphInspector inspector => m_Inspector;
        public ParameterBoard parameterBoard => m_ParameterBoard;
        private ParameterBoard m_ParameterBoard;
        private AnimationGraphInspector m_Inspector;
        private VisualElement m_Container;
        private AnimationGraphView m_AnimationGraphView;
        private StateMachineNode m_StateMachineNode;
        public StateMachineNode stateMachineNode => m_StateMachineNode;

        private StateTransition m_PreviewTransition;

        public StateNode currentSelectedNode { get; set; }
        public StateNode lastSelectedNode { get; set; }

        private StateNode m_DefaultNode;

        public StateNode defaultNode
        {
            get => m_DefaultNode;
            set
            {
                if (m_DefaultNode != null)
                {
                    m_DefaultNode.CancelDefault();
                }
                
                m_DefaultNode = value;
                
                if (m_DefaultNode != null)
                {
                    m_DefaultNode.SetDefault();
                    stateMachineNode.defaultStateId = m_DefaultNode.id;
                }
                else
                {
                    stateMachineNode.defaultStateId = 0;
                }
            }
        }

        private bool m_IsMakingTransition;

        public bool isMakingTransition
        {
            get
            {
                return m_IsMakingTransition;
            }
            set
            {
                m_IsMakingTransition = value;
                if (!m_IsMakingTransition && m_PreviewTransition != null)
                {
                    RemoveElement(m_PreviewTransition);
                    m_PreviewTransition = null;
                }
            }
        }

        public class TransitionToAdd
        {
            public StateNode source;
            public StateNode target;
        }
        
        public TransitionToAdd transitionToAdd { get; set; }

        internal StateMachineGraphView(VisualElement container, AnimationGraphView animationGraphView,
            StateMachineNode stateMachineNode, ParameterBoard parameterBoard, AnimationGraphInspector inspector)
        {
            m_Container = container;
            m_AnimationGraphView = animationGraphView;
            m_StateMachineNode = stateMachineNode;
            m_ParameterBoard = parameterBoard;
            m_Inspector = inspector;

            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
            AddDefaultManipulators();
            this.AddManipulator(CreateContextualMenu());
            AddStyleSheet();

            Button returnButton = new Button(ReturnBack);
            returnButton.Add(new Label("Return back"));
            this.Add(returnButton);

            transitionToAdd = new TransitionToAdd();

            InitializeFromStateMachineNode();
        }

        private void InitializeFromStateMachineNode()
        {
            foreach (var stateConfig in m_StateMachineNode.stateConfigs)
            {
                var stateNode = new StateNode(this.m_AnimationGraphView, this, stateConfig.position);
                stateNode.LoadFromConfig(stateConfig);
                AddElement(stateNode);
            }

            foreach (var transitionConfig in m_StateMachineNode.transitionConfigs)
            {
                var stateTransition = new StateTransition(this);
                stateTransition.LoadFromConfig(transitionConfig);
                AddElement(stateTransition);
            }

            if (m_StateMachineNode.stateConfigs.Count != 0)
            {
                defaultNode = GetNodeById(m_StateMachineNode.defaultStateId) as StateNode;
            }
        }

        private void ReturnBack()
        {
            m_AnimationGraphView.CloseStateMachineGraphView();
        }

        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(OnContextMenuPopulate);
            return contextualMenuManipulator;
        }
        
        private void OnContextMenuPopulate(ContextualMenuPopulateEvent menuEvent)
        {
            DropdownMenuAction.Status addStateMenuStatus = currentSelectedNode == null
                ? DropdownMenuAction.Status.Normal
                : DropdownMenuAction.Status.Hidden;
            
            DropdownMenuAction.Status makeTransitionMenuStatus = currentSelectedNode == null
                ? DropdownMenuAction.Status.Hidden
                : DropdownMenuAction.Status.Normal;

            DropdownMenuAction.Status setDefaultMenuStatus = makeTransitionMenuStatus;

            menuEvent.menu.AppendAction(
                "Add State",
                actionEvent => AddState(MouseToViewPosition(actionEvent.eventInfo.mousePosition)), addStateMenuStatus
            );
            
            menuEvent.menu.AppendAction(
                "Make Transition",
                actionEvent => StartMakingTransition(actionEvent), makeTransitionMenuStatus
            );
            
            menuEvent.menu.AppendAction(
                "Set Default State",
                actionEvent => SetDefaultState(), setDefaultMenuStatus
            );
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (isMakingTransition && evt.target == this)
            {
                isMakingTransition = false;
            }
        }
        
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (isMakingTransition && m_PreviewTransition != null)
            {
                m_PreviewTransition.candidatePosition = evt.mousePosition;
            }
        }
        
        private void AddStyleSheet()
        {
            var graphViewStyleSheet = Resources.Load<StyleSheet>(k_StyleSheetPrefix + "AnimationGraphView");
            if (graphViewStyleSheet != null)
            {
                styleSheets.Add(graphViewStyleSheet);
            }
        }

        private void AddState(Vector2 position)
        {
            var stateNode = new StateNode(this.m_AnimationGraphView, this, position);
            stateNode.InitializeDefault();
            AddElement(stateNode);

            if (stateMachineNode.stateConfigs.Count == 0)
            {
                defaultNode = stateNode;
            }

            m_StateMachineNode.OnAddState(stateNode.nodeConfig as StatePoseNodeConfig);
        }

        private void AddTransition()
        {
            var transition = new StateTransition(this);
            transition.InitializeDefault(transitionToAdd.source, transitionToAdd.target);
            AddElement(transition);
            stateMachineNode.OnAddTransition(transition.edgeConfig as TransitionConfig);
            transitionToAdd.source = null;
            transitionToAdd.target = null;
        }

        private void SetDefaultState()
        {
            if (currentSelectedNode != null)
            {
                defaultNode = currentSelectedNode;
            }
        }

        public void SetRandomStateAsDefault()
        {
            if (m_StateMachineNode.stateConfigs.Count > 0)
            {
                var stateNode = GetNodeById(m_StateMachineNode.stateConfigs[0].id) as StateNode;
                defaultNode = stateNode;
            }
            else
            {
                defaultNode = null;
            }
        }
        
        public void TryAddTransition()
        {
            if (transitionToAdd.source == null || transitionToAdd.target == null)
            {
                return;
            }

            if (transitionToAdd.source == transitionToAdd.target)
            {
                return;
            }

            AddTransition();
        }
        
        private void StartMakingTransition(DropdownMenuAction actionEvent)
        {
            isMakingTransition = true;
            if (m_PreviewTransition == null)
            {
                m_PreviewTransition = new StateTransition(this);
                m_PreviewTransition.InitializeDefault(currentSelectedNode, null);
                m_PreviewTransition.candidatePosition = actionEvent.eventInfo.mousePosition;
                AddElement(m_PreviewTransition);
            }
        }
    }
}
