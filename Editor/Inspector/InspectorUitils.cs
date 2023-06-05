using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class InspectorUitils
    {
        public static Label CreateHeader(string title)
        {
            Label header = new Label(title)
            {
                style =
                {
                    fontSize = 14,
                    paddingTop = 2,
                    paddingBottom = 2,
                    marginTop = 2,
                    marginBottom = 2,
                    borderBottomWidth = 1f,
                    borderBottomColor = Color.gray
                }
            };

            return header;
        }

        public static TextField CreateTextField(string label, EventCallback<ChangeEvent<string>> valueChangeCallback)
        {
            TextField textField = new TextField(label);
            textField.RegisterValueChangedCallback(valueChangeCallback);
            return textField;
        }
        
        public static IntegerField CreateIntField(string label, EventCallback<ChangeEvent<int>> valueChangeCallback)
        {
            IntegerField intField = new IntegerField(label);
            intField.RegisterValueChangedCallback(valueChangeCallback);
            return intField;
        }
        
        public static FloatField CreateFloatField(string label, EventCallback<ChangeEvent<float>> valueChangeCallback)
        {
            FloatField floatField = new FloatField(label);
            floatField.RegisterValueChangedCallback(valueChangeCallback);
            return floatField;
        }
    }
}