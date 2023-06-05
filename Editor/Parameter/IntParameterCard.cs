using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class IntParameterCard : ParameterCard
    {
        public IntParameterCard(ParameterBoard parameterBoard, string name) : base(parameterBoard, name)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Int";
        }
        
        public IntParameterCard(ParameterBoard parameterBoard, ParameterData parameterData) : base(parameterBoard, parameterData)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Int";
        }
    }
}
