using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class StateTransition
    {
        private const string k_StyleSheetPrefix = "StyleSheet/";
        
        private static CustomStyleProperty<int> s_TranstionWidthProperty = new CustomStyleProperty<int>("--edge-width");
        private static CustomStyleProperty<Color> s_TransitionEdgeColorProperty = new CustomStyleProperty<Color>("--selected-edge-color");
        private static CustomStyleProperty<Color> s_TransitionColorProperty = new CustomStyleProperty<Color>("--edge-color");

        private static readonly float s_ArrowWidth = 20;
        private const float k_MaxInterval = 10;
        private const float k_InterceptWidth = 6.0f;

        public int transitionWidth { get; set; } = s_DefaultTransitionWidth;
        public Color selectedColor { get; private set; } = s_DefaultSelectedColor;
        public Color defaultColor { get; private set; } = s_DefaultColor;
        private static readonly int s_DefaultTransitionWidth = 2;
        private static readonly Color s_DefaultSelectedColor = new Color(240 / 255f, 240 / 255f, 240 / 255f);
        private static readonly Color s_DefaultColor = new Color(146 / 255f, 146 / 255f, 146 / 255f);
        
        private TransitionControl m_TransitionControl;

        public TransitionControl transitionControl
        {
            get
            {
                if (m_TransitionControl == null)
                {
                    m_TransitionControl = CreateTransitionControl();
                }

                return m_TransitionControl;
            }
        }
        
        private TransitionControl CreateTransitionControl()
        {
            return new TransitionControl(m_StateMachineGraphView)
            {
                interceptWidth = k_InterceptWidth
            };
        }
        
        public Vector2 from
        {
            get => transitionControl.from;
            set => transitionControl.from = value;
        }
        
        public Vector2 to
        {
            get => transitionControl.to;
            set => transitionControl.to = value;
        }
        
        private Vector2 m_CandidatePosition;
        private Vector2 m_GlobalCandidatePosition;

        public Vector2 candidatePosition
        {
            get => m_CandidatePosition;
            set
            {
                if (!Approximately(m_CandidatePosition, value))
                {
                    m_CandidatePosition = value;

                    m_GlobalCandidatePosition = this.WorldToLocal(m_CandidatePosition);

                    UpdateTransitionControl();
                }
            }
        }

        protected override void OnCustomStyleResolved(ICustomStyle styles)
        {
            base.OnCustomStyleResolved(styles);

            if (styles.TryGetValue(s_TranstionWidthProperty, out var edgeWidthValue))
            {
                transitionWidth = edgeWidthValue;
            }

            if (styles.TryGetValue(s_TransitionEdgeColorProperty, out var selectColorValue))
            {
                selectedColor = selectColorValue;
            }

            if (styles.TryGetValue(s_TransitionColorProperty, out var edgeColorValue))
            {
                defaultColor = edgeColorValue;
            }

            UpdateTransitionControlColorsAndWidth();
        }
        
        private void OnTransitionAttach(AttachToPanelEvent evt)
        {
            UpdateTransitionControl();
        }

        #region UNITY_CALLS
        public override bool ContainsPoint(Vector2 localPoint)
        {
            var result = UpdateTransitionControl() &&
                         transitionControl.ContainsPoint(this.ChangeCoordinatesTo(transitionControl, localPoint));
            return result;
        }
        
        public override bool Overlaps(Rect rectangle)
        {
            if (!UpdateTransitionControl())
            {
                return false;
            }

            rectangle.height = 5f;
            rectangle.width = 5f;
            return transitionControl.Overlaps(this.ChangeCoordinatesTo(transitionControl, rectangle));
        }
        
        public override void OnSelected()
        {
            UpdateTransitionControlColorsAndWidth();
            m_StateMachineGraphView.inspector.SetCustomContent(CreateInspectorGUI());
        }

        public override void OnUnselected()
        {
            UpdateTransitionControlColorsAndWidth();
            m_StateMachineGraphView.inspector.ClearInspector();
        }
        

        #endregion

        //Update All TransitionControl Properties 
        public virtual bool UpdateTransitionControl()
        {
            if (m_SourceState == null && m_SourceState == null)
            {
                return false;
            }

            if (m_StateMachineGraphView == null)
            {
                return false;
            }

            UpdateTransitionPoints();
            transitionControl.UpdateLayout();
            UpdateTransitionControlColorsAndWidth();

            return true;
        }
        
        private void UpdateTransitionPoints()
        {
            if (m_TargetState == null && m_SourceState == null)
            {
                return;
            }
            
            if (m_TargetState == null)
            {
                transitionControl.from = m_SourceState.GetPosition().center;
                transitionControl.to = m_GlobalCandidatePosition;
            }
            else if (m_SourceState == null)
            {
                transitionControl.from = m_GlobalCandidatePosition;
                transitionControl.to = m_TargetState.GetPosition().center;
            }
            else
            {
                ComputeControlPoints();
            }
        }
        
        private void UpdateTransitionControlColorsAndWidth()
        {
            if (selected)
            {
                transitionControl.color = selectedColor;
                transitionControl.transitionWidth = transitionWidth;
            }
            else
            {
                transitionControl.color = defaultColor;
                transitionControl.transitionWidth = transitionWidth;
            }
        }
        
        private void ComputeControlPoints()
        {
            var inputTransitionCount = 0;
            var outputTransitionCount = 0;
            var index = -1;

            foreach (var transition in m_SourceState.outputTransitions)
            {
                if (transition.m_TargetState == m_TargetState)
                {
                    if (ReferenceEquals(this, transition))
                    {
                        index = outputTransitionCount;
                    }

                    outputTransitionCount++;
                }
            }
            
            foreach (var transition in m_TargetState.outputTransitions)
            {
                if (transition.m_TargetState == m_SourceState)
                {
                    if (ReferenceEquals(this, transition))
                    {
                        index = outputTransitionCount;
                    }

                    outputTransitionCount++;
                }
            }

            if (index == -1)
            {
                return;
            }

            var sourceCenter = m_SourceState.GetPosition().center;
            var destinationCenter = m_TargetState.GetPosition().center;

            var sourceToDestination = destinationCenter - sourceCenter;
            var normal = new Vector2(-sourceToDestination.y, sourceToDestination.x).normalized;

            var totalCount = inputTransitionCount + outputTransitionCount;

            var radius = m_SourceState.GetPosition().height * 0.5f;
            var length = k_MaxInterval * (totalCount - 1) * 0.5f;
            if (length > radius - 2)
            {
                length = radius - 2;
            }

            var fraction = totalCount == 1 ? 0 : (float)index / (totalCount - 1) * 2f - 1f; // [0,1] -> [-1, 1]
            var p1 = sourceCenter + normal * length * fraction;
            var p2 = destinationCenter + normal * length * fraction;

            transitionControl.from = p1;
            transitionControl.to = p2;
        }

        private static bool Approximately(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
        }
    }
}