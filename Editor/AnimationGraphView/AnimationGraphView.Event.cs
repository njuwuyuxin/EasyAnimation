using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        private void RegisterCallbacks()
        {
            RegisterCallback<DragEnterEvent>(OnDragEnter);
            RegisterCallback<DragLeaveEvent>(OnDragLeave);
            RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
            RegisterCallback<DragPerformEvent>(OnDragPerform);
            RegisterCallback<DragExitedEvent>(OnDragExit);
            RegisterCallback<KeyDownEvent>(OnKeyDown);
        }

        private void OnDragEnter(DragEnterEvent evt)
        {
        }
        
        private void OnDragLeave(DragLeaveEvent evt)
        {
            
        }
        
        private void OnDragUpdate(DragUpdatedEvent evt)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
        }
        
        private void OnDragPerform(DragPerformEvent evt)
        {
            DragAndDrop.AcceptDrag();
            
            //Drag Fbx or AnimationClip to create AnimationClipNode
            if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
            {
                foreach (var objectReference in DragAndDrop.objectReferences)
                {
                    var assetPath = AssetDatabase.GetAssetPath(objectReference);
                    if (assetPath != null)
                    {
                        AnimationClip clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
                        if (clip != null)
                        {
                            var animtionClipNode = CreateDefaultNode(ENodeType.AnimationClipNode, MouseToViewPosition(evt.mousePosition));
                            (animtionClipNode.nodeConfig as AnimationClipPoseNodeConfig).clip = clip;
                        }
                    }
                }

                return;
            }
            
            //Drag ParameterCard to create ValueNode
            var parameterCard = DragAndDrop.GetGenericData("parameterCard") as ParameterCard;
            if (parameterCard != null)
            {
                CreateParameterNode(parameterCard, MouseToViewPosition(evt.mousePosition));
                Debug.Log(parameterCard.parameterName);
            }
        }
        
        private void OnDragExit(DragExitedEvent evt)
        { 
            
        }

        private void OnKeyDown(KeyDownEvent keyDownEvent)
        {
            if (keyDownEvent.keyCode == KeyCode.Z && keyDownEvent.modifiers == EventModifiers.Control)
            {
                TryUndoCommand();
                keyDownEvent.StopPropagation();
            }
            
            if (keyDownEvent.keyCode == KeyCode.Y && keyDownEvent.modifiers == EventModifiers.Control)
            {
                TryRedoCommand();
                keyDownEvent.StopPropagation();
            }

            if (keyDownEvent.keyCode == KeyCode.Delete && selection != null)
            {
                var batchDeleteCommand = new BatchDeleteCommand(this);
                PushNewCommand(batchDeleteCommand);
                keyDownEvent.StopPropagation();
            }
        }
    }
}
