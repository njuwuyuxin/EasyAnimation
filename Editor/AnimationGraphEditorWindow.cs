using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class AnimationGraphEditorWindow : EditorWindow
    {
        private VisualElement m_ParameterArea;
        private VisualElement m_GraphArea;
        private VisualElement m_InspectorArea;
        private AnimationGraphInspector m_Inspector;
        // private IMGUIInspector m_IMGUIInspector;
        private ParameterBoard m_ParameterBoard;

        private const string k_StyleSheetPrefix = "StyleSheet/";
        private const string k_CompiledGraphSavePath = "Assets/Data/AnimationGraph/";

        private AnimationGraphAsset m_AnimationGraphAsset;
        private AnimationGraphView m_AnimationGraphView;
        
        
        [MenuItem("Window/Easy Animation/AnimationGraphEditorWindow")]
        public static void ShowWindow()
        {
            ShowWindow(null);
        }
        
        public static void ShowWindow(AnimationGraphAsset animationGraphAsset)
        {
            AnimationGraphEditorWindow wnd = GetWindow<AnimationGraphEditorWindow>();
            wnd.titleContent = new GUIContent("AnimationGraphEditorWindow");

            if (animationGraphAsset != null)
            {
                wnd.ClearAnimationGraphWindow();
                wnd.LoadAnimationGraphAsset(animationGraphAsset);
            }
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var mainWindowVisualTree = Resources.Load<VisualTreeAsset>("UIDocuments/AnimationGraphEditorWindow");
            VisualElement mainWindowElement = mainWindowVisualTree.CloneTree();
            mainWindowElement.style.flexGrow = 1;

            var editorWindowStyleSheet = Resources.Load<StyleSheet>(k_StyleSheetPrefix + "AnimationGraphEditorWindow");
            if (editorWindowStyleSheet != null)
            {
                mainWindowElement.styleSheets.Add(editorWindowStyleSheet);
            }

            root.Add(mainWindowElement);
            
            Button saveButton = new Button(Save);
            saveButton.Add(new Label("Save"));
            mainWindowElement.Add(saveButton);
            
            Button compileButton = new Button(CompileGraph);
            compileButton.Add(new Label("Compile"));
            mainWindowElement.Add(compileButton);
            
            m_GraphArea = mainWindowElement.Q("GraphArea");
            m_ParameterArea = mainWindowElement.Q("ParameterArea");

            m_ParameterBoard = new ParameterBoard();
            m_ParameterArea.Add(m_ParameterBoard);
            m_ParameterBoard.StretchToParentSize();
            
            m_InspectorArea = mainWindowElement.Q("InspectorArea");
            m_Inspector = new AnimationGraphInspector();
            m_InspectorArea.Add(m_Inspector);
            m_Inspector.StretchToParentSize();

            InitAnimGraphView();
        }

        private void InitAnimGraphView()
        {
            m_AnimationGraphView = new AnimationGraphView(m_GraphArea, m_ParameterBoard, m_Inspector);
            m_GraphArea.Add(m_AnimationGraphView);
            m_AnimationGraphView.StretchToParentSize();
        }

        public void OnDestroy()
        {
            m_ParameterBoard.OnDestroy();
            m_AnimationGraphView.OnDestory();
        }

        private void ClearAnimationGraphWindow()
        {
            if (m_ParameterBoard != null)
            {
                m_ParameterBoard.ClearParameterBoard();
            }

            if (m_AnimationGraphView != null)
            {
                m_AnimationGraphView.ClearAnimationGraphView();
            }
        }
        
        private void LoadAnimationGraphAsset(AnimationGraphAsset graphAsset)
        {
            m_AnimationGraphAsset = graphAsset;
            m_ParameterBoard.LoadAnimGraphAsset(graphAsset);
            m_AnimationGraphView.LoadAnimGraphAsset(m_AnimationGraphAsset);
        }

        private void Save()
        {
            m_ParameterBoard.Save();
            m_AnimationGraphView.Save();
            EditorUtility.DisplayDialog("Success", "Animation Graph Save Successfully!", "OK");
        }

        private void CompileGraph()
        {
            CompiledAnimationGraph compiledGraph = null;
            string savePath = k_CompiledGraphSavePath + m_AnimationGraphAsset.name + "_Compiled.asset";
            if (!System.IO.Directory.Exists(k_CompiledGraphSavePath))
            {
                System.IO.Directory.CreateDirectory(k_CompiledGraphSavePath);
            }
            
            if (System.IO.File.Exists(savePath))
            {
                compiledGraph = AssetDatabase.LoadAssetAtPath<CompiledAnimationGraph>(savePath);
                m_AnimationGraphView.Compile(compiledGraph);
                m_ParameterBoard.Compile(compiledGraph);
                EditorUtility.SetDirty(compiledGraph);
                AssetDatabase.SaveAssets();
            }
            else
            {
                compiledGraph = CreateInstance<CompiledAnimationGraph>();
                m_AnimationGraphView.Compile(compiledGraph);
                m_ParameterBoard.Compile(compiledGraph);
                AssetDatabase.CreateAsset(compiledGraph, savePath);
            }
            
            EditorUtility.DisplayDialog("Success", "Animation Graph Compile Successfully!", "OK");

        }
    }
}