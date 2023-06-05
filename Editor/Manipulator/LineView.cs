using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
  internal class LineView : VisualElement
  {
    // internal static PrefColor s_SnappingLineColor = new PrefColor("General/Graph Snapping Line Color", 0.26666668f, 0.7529412f, 1f, 0.2f);

    public List<Line2> lines { get; private set; } = new List<Line2>();

    public LineView()
    {
      this.StretchToParentSize();
      this.generateVisualContent = this.generateVisualContent + new Action<MeshGenerationContext>(this.OnGenerateVisualContent);
    }

    private void OnGenerateVisualContent(MeshGenerationContext mgc)
    {
      UnityEditor.Experimental.GraphView.GraphView firstAncestorOfType = this.GetFirstAncestorOfType<UnityEditor.Experimental.GraphView.GraphView>();
      if (firstAncestorOfType == null)
        return;
      VisualElement contentViewContainer = firstAncestorOfType.contentViewContainer;
      foreach (Line2 line in this.lines)
      {
        Vector2 vector2_1 = contentViewContainer.ChangeCoordinatesTo((VisualElement) firstAncestorOfType, line.start);
        Vector2 vector2_2 = contentViewContainer.ChangeCoordinatesTo((VisualElement) firstAncestorOfType, line.end);
        Rect rect = new Rect(Math.Min(vector2_1.x, vector2_2.x), Math.Min(vector2_1.y, vector2_2.y), Math.Max(1f, Math.Abs(vector2_1.x - vector2_2.x)), Math.Max(1f, Math.Abs(vector2_1.y - vector2_2.y)));
        // mgc.Rectangle(MeshGenerationContextUtils.RectangleParams.MakeSolid(rect, (Color) LineView.s_SnappingLineColor, ContextType.Editor));
      }
    }
  }
}
