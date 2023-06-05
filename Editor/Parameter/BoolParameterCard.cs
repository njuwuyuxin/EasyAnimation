using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class BoolParameterCard : ParameterCard
    {
        public BoolParameterCard(ParameterBoard parameterBoard, string name) : base(parameterBoard, name)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Bool";
        }
        
        public BoolParameterCard(ParameterBoard parameterBoard, ParameterData parameterData) : base(parameterBoard, parameterData)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Bool";
        }
    }
}
