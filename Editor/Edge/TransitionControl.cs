using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor.Experimental.GraphView;

namespace AnimationGraph.Editor
{
    public class TransitionControl : VisualElement
    {

        private static readonly float s_ArrowCosAngle = Mathf.Cos(60);
        private static readonly float s_ArrowSinAngle = Mathf.Sin(60);
        private static readonly float s_ArrowWidth = 20;

        private static PropertyInfo s_LayoutProperty = typeof(VisualElement).GetProperty("layout", BindingFlags.Instance | BindingFlags.Public);
        
        private GraphView m_GraphView;
        
        private readonly List<Vector2> m_RenderPoints = new List<Vector2>();
        private readonly List<Vector2> m_LastRenderPoints = new List<Vector2>();
        
        private bool m_PointsDirty = true;
        private bool m_RenderPointsDirty = true;
        
        private const float k_MinTransitionWidth = 1.75f;
        private const float k_ArrowLength = 4f;

        private Color m_Color;
        public Color color
        {
            get => m_Color;
            set
            {
                if (m_Color == value)
                {
                    return;
                }

                m_Color = value;
                MarkDirtyRepaint();
            }
        }
        
        private int m_TransitionWidth = 2;
        public int transitionWidth
        {
            get => m_TransitionWidth;
            set
            {
                if (m_TransitionWidth == value)
                {
                    return;
                }

                m_TransitionWidth = value;
                UpdateLayout();
                MarkDirtyRepaint();
            }
        }

        public float interceptWidth { get; set; } = 5;

        // The start of the transition in graph coordinates.
        private Vector2 m_From;
        public Vector2 from
        {
            get => m_From;
            set
            {
                if (!((m_From - value).sqrMagnitude > 0.25f))
                {
                    return;
                }

                m_From = value;
                PointsChanged();
            }
        }

        // The end of the transition in graph coordinates.
        private Vector2 m_To;
        public Vector2 to
        {
            get => m_To;
            set
            {
                if (!((m_To - value).sqrMagnitude > 0.25f))
                {
                    return;
                }

                m_To = value;
                PointsChanged();
            }
        }
        
        public TransitionControl(StateMachineGraphView graphView)
        {
            m_GraphView = graphView;
            pickingMode = PickingMode.Ignore;

            generateVisualContent += OnGenerateVisualContent;
        }
        
        private void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            DrawTransition(mgc);
        }

        public override bool ContainsPoint(Vector2 localPoint)
        {
            if (!base.ContainsPoint(localPoint))
            {
                return false;
            }
            
            const float maxDist = k_ArrowLength * k_ArrowLength;
            if ((from - localPoint).sqrMagnitude <= maxDist || (to - localPoint).sqrMagnitude <= maxDist)
            {
                return false;
            }

            var allPoints = m_RenderPoints;
            if (allPoints.Count < 2)
            {
                return false;
            }
            
            var currentPoint = allPoints[0];
            var nextPoint = allPoints[1];
            var distance = (currentPoint - localPoint).sqrMagnitude;
            var interceptWidth2 = interceptWidth * interceptWidth;
            
            var next2Current = nextPoint - currentPoint;
            var distanceNext = (nextPoint - localPoint).sqrMagnitude;
            var distanceLine = next2Current.sqrMagnitude;
                
            if (distance < distanceLine && distanceNext < distanceLine)
            {
                var d = next2Current.y * localPoint.x -
                        next2Current.x * localPoint.y + nextPoint.x * currentPoint.y -
                        nextPoint.y * currentPoint.x;
                if (d * d < interceptWidth2 * distanceLine)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Overlaps(Rect rect)
        {
            if (base.Overlaps(rect) && m_RenderPoints.Count > 0)
            {
                if (RectUtils.IntersectsSegment(rect, m_RenderPoints[0], m_RenderPoints[1]))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void PointsChanged()
        {
            m_PointsDirty = true;
            MarkDirtyRepaint();
        }
        
        private static bool Approximately(Vector2 v1, Vector2 v2)
        {
            return Mathf.Approximately(v1.x, v2.x) && Mathf.Approximately(v1.y, v2.y);
        }

        public void UpdateLayout()
        {
            if (parent == null)
            {
                return;
            }

            if (m_PointsDirty)
            {
                ComputeLayout();
                m_PointsDirty = false;
            }
            MarkDirtyRepaint();
        }
        
        private void RenderArrow(Vector2 p1, Vector2 p2)
        {
            if (Approximately(p1, p2))
            {
                return;
            }
            
            // line
            m_RenderPoints.Add(p1);
            m_RenderPoints.Add(p2);
            
            // arrow
            var v = p2 - p1;
            v *= s_ArrowWidth / v.magnitude;
            var v1 = new Vector2(v.x * s_ArrowCosAngle - v.y * s_ArrowSinAngle, v.x * s_ArrowSinAngle + v.y * s_ArrowCosAngle);
            var v2 = new Vector2(v.x * s_ArrowCosAngle + v.y * s_ArrowSinAngle, v.x * -s_ArrowSinAngle + v.y * s_ArrowCosAngle);
            
            var middle = (p1 + p2) / 2;
            m_RenderPoints.Add(middle);
            m_RenderPoints.Add(middle + (p1 - p2).normalized * s_ArrowWidth * 0.5f);
            m_RenderPoints.Add(middle + v1);
            m_RenderPoints.Add(middle + v2);

        }


        protected virtual void UpdateRenderPoints()
        {
            if (m_RenderPointsDirty == false)
            {
                return;
            }

            var p1 = parent.ChangeCoordinatesTo(this, m_From);
            var p2 = parent.ChangeCoordinatesTo(this, m_To);

            // Only compute this when the "local" points have actually changed
            if (m_LastRenderPoints.Count == 4)
            {
                if (Approximately(p1, m_LastRenderPoints[0]) &&
                    Approximately(p2, m_LastRenderPoints[1]))
                {
                    m_RenderPointsDirty = false;
                    return;
                }
            }
            
            m_LastRenderPoints.Clear();
            m_LastRenderPoints.Add(p1);
            m_LastRenderPoints.Add(p2);
            m_RenderPointsDirty = false;

            m_RenderPoints.Clear();
            RenderArrow(p1, p2);
        }
        
        private void ComputeLayout()
        {
            var rect = new Rect(Vector2.Min(m_To, m_From), new Vector2(Mathf.Abs(from.x - to.x), Mathf.Abs(from.y - to.y)));
            
            if (!Approximately(m_To, m_From))
            {
                var v = m_To - m_From;
                v *= s_ArrowWidth / v.magnitude;
                var v1 = new Vector2(v.x * s_ArrowCosAngle - v.y * s_ArrowSinAngle, v.x * s_ArrowSinAngle + v.y * s_ArrowCosAngle);
                var v2 = new Vector2(v.x * s_ArrowCosAngle + v.y * s_ArrowSinAngle, v.x * -s_ArrowSinAngle + v.y * s_ArrowCosAngle);
                var p1 = m_To + v1;
                var p2 = m_To + v2;
            
                if (!rect.Contains(p1))
                {
                    rect.xMin = Math.Min(rect.xMin, p1.x);
                    rect.yMin = Math.Min(rect.yMin, p1.y);
                    rect.xMax = Math.Max(rect.xMax, p1.x);
                    rect.yMax = Math.Max(rect.yMax, p1.y);
                }
            
                if (!rect.Contains(p2))
                {
                    rect.xMin = Math.Min(rect.xMin, p2.x);
                    rect.yMin = Math.Min(rect.yMin, p2.y);
                    rect.xMax = Math.Max(rect.xMax, p2.x);
                    rect.yMax = Math.Max(rect.yMax, p2.y);
                }
            }

            if (m_GraphView == null)
            {
                m_GraphView = GetFirstAncestorOfType<GraphView>();
            }
            
            var margin = Mathf.Max(transitionWidth * 0.5f + 1, k_MinTransitionWidth / m_GraphView.minScale);

            rect.xMin -= margin;
            rect.yMin -= margin;
            rect.width += margin;
            rect.height += margin;

            if (layout != rect)
            {
                s_LayoutProperty.SetValue(this, rect);
                m_RenderPointsDirty = true;
            }
        }

        private void DrawTransition(MeshGenerationContext mgc)
        {
            if (transitionWidth <= 0)
            {
                return;
            }

            UpdateRenderPoints();
            
            if (m_RenderPoints.Count == 0)
            {
                return;
            }

            var md = mgc.Allocate(8, 12, null);
            if (md.vertexCount == 0)
            {
                return;
            }

            // setup line
            var halfWidth = transitionWidth * 0.5f;
            
            var p0 = m_RenderPoints[0];
            var p1 = m_RenderPoints[1];
            
            var v = p1 - p0;
            v *= halfWidth / v.magnitude;
            v = new Vector2(-v.y, v.x);

            md.SetNextVertex(new Vertex { position = p0 + v, tint = color });
            md.SetNextVertex(new Vertex { position = p0 - v, tint = color });
            md.SetNextVertex(new Vertex { position = p1 + v, tint = color });
            md.SetNextVertex(new Vertex { position = p1 - v, tint = color });
            
            md.SetNextIndex(0);
            md.SetNextIndex(1);
            md.SetNextIndex(2);
            md.SetNextIndex(1);
            md.SetNextIndex(3);
            md.SetNextIndex(2);

            // Setup arrow
            md.SetNextVertex(new Vertex { position = m_RenderPoints[2], tint = color });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[3], tint = color });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[4], tint = color });
            md.SetNextVertex(new Vertex { position = m_RenderPoints[5], tint = color });

            md.SetNextIndex(4);
            md.SetNextIndex(5);
            md.SetNextIndex(6);
            md.SetNextIndex(4);
            md.SetNextIndex(7);
            md.SetNextIndex(5);
        }
    }
}
