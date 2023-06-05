using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
  internal class Snapper
  {
    private SnapService m_Service;
    private LineView m_LineView;
    private UnityEditor.Experimental.GraphView.GraphView m_GraphView;

    private bool active => this.m_Service != null && this.m_Service.active;

    internal void BeginSnap(UnityEditor.Experimental.GraphView.GraphView graphView)
    {
      if (this.m_Service == null)
        this.m_Service = new SnapService();
      if (this.m_LineView == null)
        this.m_LineView = new LineView();
      this.m_GraphView = graphView;
      this.m_GraphView.Add((VisualElement) this.m_LineView);
      LineView lineView = this.m_LineView;
      Rect layout = this.m_GraphView.layout;
      double width = (double) layout.width;
      layout = this.m_GraphView.layout;
      double height = (double) layout.height;
      Rect rect = new Rect(0.0f, 0.0f, (float) width, (float) height);
      // lineView.layout = rect;
      this.m_Service.BeginSnap(this.GetNotSelectedElementRectsInView());
    }

    internal List<Rect> GetNotSelectedElementRectsInView()
    {
      List<Rect> elementRectsInView = new List<Rect>();
      List<GraphElement> graphElementList1 = new List<GraphElement>();
      foreach (GraphElement graphElement in this.m_GraphView.selection.OfType<GraphElement>())
      {
        UQueryBuilder<GraphElement> uqueryBuilder;
        if (graphElement is Group)
        {
          foreach (GraphElement containedElement in (graphElement as Group).containedElements)
          {
            List<GraphElement> graphElementList2 = graphElementList1;
            uqueryBuilder = containedElement.Query<GraphElement>("", "graphElement");
            List<GraphElement> list = uqueryBuilder.ToList();
            graphElementList2.AddRange((IEnumerable<GraphElement>) list);
          }
        }
        else if (graphElement.GetContainingScope() != null)
          graphElementList1.Add((GraphElement) graphElement.GetContainingScope());
        else
          graphElementList1.Add(graphElement);
        List<GraphElement> graphElementList3 = graphElementList1;
        uqueryBuilder = graphElement.Query<GraphElement>("", "graphElement");
        List<GraphElement> list1 = uqueryBuilder.ToList();
        graphElementList3.AddRange((IEnumerable<GraphElement>) list1);
      }
      Rect layout = this.m_GraphView.layout;
      List<ISelectable> selectableList = new List<ISelectable>();
      foreach (GraphElement dest in this.m_GraphView.graphElements.ToList())
      {
        Rect rectangle = this.m_GraphView.ChangeCoordinatesTo((VisualElement) dest, layout);
        if (dest.Overlaps(rectangle))
          selectableList.Add((ISelectable) dest);
      }
      foreach (GraphElement graphElement in selectableList)
      {
        if (!graphElement.IsSelected((VisualElement) this.m_GraphView) && (graphElement.capabilities & Capabilities.Snappable) != (Capabilities) 0 && !graphElementList1.Contains(graphElement))
        {
          Rect rect = graphElement.parent.ChangeCoordinatesTo(this.m_GraphView.contentViewContainer, graphElement.GetPosition());
          elementRectsInView.Add(rect);
        }
      }
      return elementRectsInView;
    }

    internal Rect GetSnappedRect(Rect sourceRect, float scale = 1f)
    {
      this.m_Service.UpdateSnapRects(this.GetNotSelectedElementRectsInView());
      List<SnapResult> results;
      Rect snappedRect = this.m_Service.GetSnappedRect(sourceRect, out results, scale);
      this.m_LineView.lines.Clear();
      foreach (SnapResult snapResult in results)
        this.m_LineView.lines.Add(snapResult.indicatorLine);
      this.m_LineView.MarkDirtyRepaint();
      return snappedRect;
    }

    internal void EndSnap(UnityEditor.Experimental.GraphView.GraphView graphView)
    {
      this.m_LineView.lines.Clear();
      this.m_LineView.Clear();
      this.m_LineView.RemoveFromHierarchy();
      this.m_Service.EndSnap();
    }

    internal void ClearSnapLines()
    {
      this.m_LineView.lines.Clear();
      this.m_LineView.MarkDirtyRepaint();
    }
  }
}
