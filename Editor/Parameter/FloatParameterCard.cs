using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class FloatParameterCard : ParameterCard
    {
        public FloatParameterCard(ParameterBoard parameterBoard, string name) : base(parameterBoard, name)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Float";
        }
        
        public FloatParameterCard(ParameterBoard parameterBoard, ParameterData parameterData) : base(parameterBoard, parameterData)
        {
            var typeLabel = m_ParameterCardTemplateContainer.Q<Label>("ParameterType");
            typeLabel.text = "Float";
        }
    }
}
