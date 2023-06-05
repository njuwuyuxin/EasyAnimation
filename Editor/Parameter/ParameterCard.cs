using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ParameterCard : VisualElement
    {
        private enum EDragState
        {
            Idle,
            Start,
            Dragging,
        }

        private EDragState m_DragStatus;

        public int id { get; private set; }
        public string parameterName
        {
            get => m_Name;
            set
            {
                m_Name = value;
                m_NameLabel.text = m_Name;
            }
        }
        protected string m_Name;
        public List<int> associatedNodes;

        private Label m_NameLabel;
        protected ParameterBoard m_ParameterBoard;
        protected TemplateContainer m_ParameterCardTemplateContainer;

        public ParameterCard(ParameterBoard parameterBoard, string name)
        {
            Initialize(parameterBoard, name);
            id = Animator.StringToHash(Guid.NewGuid().ToString());
            associatedNodes = new List<int>();
        }

        public ParameterCard(ParameterBoard parameterBoard, ParameterData parameterData)
        {
            Initialize(parameterBoard, parameterData.name);
            id = parameterData.id;
            associatedNodes = new List<int>(parameterData.associateNodes.ToArray());
        }

        private void Initialize(ParameterBoard parameterBoard, string name)
        {
            m_ParameterBoard = parameterBoard;
            
            
            var parameterCardVisualTree = Resources.Load<VisualTreeAsset>("UIDocuments/ParameterCard");
            m_ParameterCardTemplateContainer = parameterCardVisualTree.CloneTree();
            m_ParameterCardTemplateContainer.style.flexGrow = 1;
            Add(m_ParameterCardTemplateContainer);

            var styleSheet = Resources.Load<StyleSheet>("StyleSheet/ParameterCardStyleSheet");
            styleSheets.Add(styleSheet);
            
            m_NameLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterName");
            parameterName = name;
            
            this.AddManipulator(CreateContextualMenu());
            RegisterCallbacks();
        }

        private void RegisterCallbacks()
        {
            m_NameLabel.RegisterCallback<ClickEvent>(OnNameLabelClicked);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
            RegisterCallback<MouseMoveEvent>(OnMouseMove);
        }
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            m_DragStatus = EDragState.Start;
        }
        
        private void OnMouseUp(MouseUpEvent evt)
        {
            m_DragStatus = EDragState.Idle;
        }
        
        private void OnMouseMove(MouseMoveEvent evt)
        {
            if (m_DragStatus == EDragState.Start)
            {
                m_DragStatus = EDragState.Dragging;
                DragAndDrop.SetGenericData("parameterCard", this);
                DragAndDrop.StartDrag("Dragging Parameter Card");
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
        }

        private void OnNameLabelClicked(ClickEvent evt)
        {
            if (evt.clickCount == 2)
            {
                m_NameLabel.visible = false;
                TextField textField = new TextField();
                textField.value = m_NameLabel.text;
                textField.style.position = new StyleEnum<Position>(Position.Absolute);
                textField.style.height = new StyleLength(new Length(26));
                textField.style.marginTop = new StyleLength(new Length(8));
                textField.style.marginLeft = new StyleLength(new Length(10));
                textField.RegisterCallback<FocusOutEvent>(focusEvt =>
                {
                    parameterName = textField.text;
                    Remove(textField);
                    m_NameLabel.visible = true;
                });
                Add(textField);
                schedule.Execute(() =>
                {
                    textField.Focus();
                    textField.SelectAll();
                });
            }
        }
        
        private IManipulator CreateContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent =>
                {
                    menuEvent.menu.AppendAction(
                        "Delete",
                        actionEvent => Delete()
                    );
                });
            return contextualMenuManipulator;
        }

        public void Delete()
        {
            m_ParameterBoard.DeleteParameterCard(this);
        } 
    }
}
