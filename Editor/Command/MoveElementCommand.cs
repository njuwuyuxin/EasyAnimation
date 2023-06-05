using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
    public class MoveElementCommand : ICommand
    {
        private AnimationGraphView m_AnimationGraphView;
        private Dictionary<GraphElement, Rect> m_OriginalPosDict;
        private Dictionary<GraphElement, Rect> m_NewPosDict;

        internal MoveElementCommand(AnimationGraphView animationGraphView, Dictionary<GraphElement, AnimationGraphSelectionDragger.OriginalPos> originalPosDict)
        {
            m_AnimationGraphView = animationGraphView;
            m_OriginalPosDict = new Dictionary<GraphElement, Rect>();
            m_NewPosDict = new Dictionary<GraphElement, Rect>();
            foreach (var pair in originalPosDict)
            {
                m_OriginalPosDict.Add(pair.Key, pair.Value.pos);
                var newPos = pair.Key.hierarchy.parent.ChangeCoordinatesTo(m_AnimationGraphView.contentViewContainer, pair.Key.GetPosition());
                m_NewPosDict.Add(pair.Key, newPos);
            }
        }
        
        public void Do()
        {
            // Moving operation is already completed in SelectionDragger
        }

        public void Undo()
        {
            foreach (var pair in m_OriginalPosDict)
            {
                pair.Key.SetPosition(pair.Value);
            }
        }

        public void Redo()
        {
            foreach (var pair in m_NewPosDict)
            {
                pair.Key.SetPosition(pair.Value);
            }
        }
    }
}
