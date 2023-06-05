using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class ParameterBoard : VisualElement
    {
        private Button m_AddParameterButton;
        private AnimationGraphAsset m_AnimationGraphAsset;
        private ToolbarMenu m_AddParameterToolBar;
        private VisualElement m_ToolBarArea;
        private VisualElement m_ParameterArea;
        private List<ParameterCard> m_ParameterCards;

        public List<ParameterCard> parameterCards => m_ParameterCards;
        public ParameterBoard()
        {
            var parameterBoardVisualTree = Resources.Load<VisualTreeAsset>("UIDocuments/ParameterBoard");
            VisualElement parameterBoardElement = parameterBoardVisualTree.CloneTree();
            Add(parameterBoardElement);
            style.flexGrow = 1;

            m_ToolBarArea = parameterBoardElement.Q("ToolBarArea");
            m_AddParameterToolBar = new ToolbarMenu();
            m_AddParameterToolBar.text = "Add Parameter";
            m_AddParameterToolBar.menu.AppendAction(
                "Add Bool Parameter",
                actionEvent => CreateDefaultBoolParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add Int Parameter",
                actionEvent => CreateDefaultIntParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add Float Parameter",
                actionEvent => CreateDefaultFloatParameter()
            );
            m_AddParameterToolBar.menu.AppendAction(
                "Add String Parameter",
                actionEvent => CreateDefaultStringParameter()
            );
            
            m_ToolBarArea.Add(m_AddParameterToolBar);

            m_ParameterArea = parameterBoardElement.Q("ParameterArea");
            m_ParameterCards = new List<ParameterCard>();
        }

        public void ClearParameterBoard()
        {
            foreach (var parameterCard in m_ParameterCards)
            {
                m_ParameterArea.Remove(parameterCard);
            }
            m_ParameterCards.Clear();
        }
        
        public void LoadAnimGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            foreach (var parameterData in m_AnimationGraphAsset.parameters)
            {
                LoadParameterCard(parameterData);
            }
        }

        public void Save()
        {
            m_AnimationGraphAsset.parameters.Clear();
            foreach (var parameterCard in m_ParameterCards)
            {
                ParameterData parameterData = null;
                if (parameterCard is BoolParameterCard)
                {
                    parameterData = new BoolParameterData();
                }
                else if (parameterCard is IntParameterCard)
                {
                    parameterData = new IntParameterData();
                }
                else if (parameterCard is FloatParameterCard)
                {
                    parameterData = new FloatParameterData();
                }
                else if (parameterCard is StringParameterCard)
                {
                    parameterData = new StringParameterData();
                }
                else
                {
                    parameterData = new ParameterData();
                }
                parameterData.name = parameterCard.parameterName;
                parameterData.id = parameterCard.id;
                parameterData.associateNodes = new List<int>(parameterCard.associatedNodes.ToArray());
                m_AnimationGraphAsset.parameters.Add(parameterData);
            }
        }

        private void LoadParameterCard(ParameterData parameterData)
        {
            ParameterCard parameterCard = null;
            if (parameterData is BoolParameterData)
            {
                parameterCard = new BoolParameterCard(this, parameterData);
            }
            else if (parameterData is IntParameterData)
            {
                parameterCard = new IntParameterCard(this, parameterData);
            }
            else if (parameterData is FloatParameterData)
            {
                parameterCard = new FloatParameterCard(this, parameterData);
            }
            else if (parameterData is StringParameterData)
            {
                parameterCard = new StringParameterCard(this, parameterData);
            }
            else
            {
                parameterCard = new ParameterCard(this, parameterData);
            }
            
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }

        public void Compile(CompiledAnimationGraph compiledGraph)
        {
            compiledGraph.parameters = new List<GraphParameter>();
            foreach (var parameterCard in m_ParameterCards)
            {
                GraphParameter graphParameter;
                if (parameterCard is BoolParameterCard)
                {
                    graphParameter = new BoolParameter();
                }
                else if (parameterCard is IntParameterCard)
                {
                    graphParameter = new IntParameter();
                }
                else if (parameterCard is FloatParameterCard)
                {
                    graphParameter = new FloatParameter();
                }
                else if (parameterCard is StringParameterCard)
                {
                    graphParameter = new StringParameter();
                }
                else
                {
                    graphParameter = new GraphParameter();
                }

                graphParameter.id = parameterCard.id;
                graphParameter.name = parameterCard.parameterName;
                graphParameter.associatedNodes = parameterCard.associatedNodes;
                compiledGraph.parameters.Add(graphParameter);
            }
        }

        public void OnDestroy()
        {
            
        }

        public void DeleteParameterCard(ParameterCard parameterCard)
        {
            m_ParameterCards.Remove(parameterCard);
            m_ParameterArea.Remove(parameterCard);
        }

        private void CreateDefaultBoolParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("boolParameter");
            BoolParameterCard parameterCard = new BoolParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultIntParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("intParameter");
            IntParameterCard parameterCard = new IntParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultFloatParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("floatParameter");
            FloatParameterCard parameterCard = new FloatParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }
        
        private void CreateDefaultStringParameter()
        {
            string defaultParameterName = GenerateDefaultParameterName("stringParameter");
            StringParameterCard parameterCard = new StringParameterCard(this, defaultParameterName);
            m_ParameterCards.Add(parameterCard);
            m_ParameterArea.Add(parameterCard);
        }

        private string GenerateDefaultParameterName(string defaultName)
        {
            foreach (var parameterCard in m_ParameterCards)
            {
                if (parameterCard.parameterName.Equals(defaultName))
                {
                    return GenerateDefaultParameterName(defaultName + "(copy)");
                }
            }

            return defaultName;
        }

        public ParameterCard TryGetParameterById(int id)
        {
            return m_ParameterCards.Find(m => m.id == id);
        }
    }
}