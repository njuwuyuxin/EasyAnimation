using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AnimationGraph.Editor
{
  internal enum SnapReference
  {
    LeftEdge,
    HorizontalCenter,
    RightEdge,
    TopEdge,
    VerticalCenter,
    BottomEdge,
  }

  internal struct Line2
  {
    public Vector2 start { get; set; }

    public Vector2 end { get; set; }

    public Line2(Vector2 start, Vector2 end)
    {
      this.start = start;
      this.end = end;
    }
  }

  internal class SnapResult
  {
    public Line2 indicatorLine;

    public Rect sourceRect { get; set; }

    public SnapReference sourceReference { get; set; }

    public Rect snappableRect { get; set; }

    public SnapReference snappableReference { get; set; }

    public float offset { get; set; }

    public float distance => Math.Abs(this.offset);
  }

  internal class SnapService
  {
    private const float k_DefaultSnapDistance = 8f;
    private float m_CurrentScale = 1f;
    private List<Rect> m_SnappableRects = new List<Rect>();

    public bool active { get; private set; }

    public float snapDistance { get; set; }

    public SnapService() => this.snapDistance = 8f;

    internal static float GetPos(Rect rect, SnapReference reference)
    {
      switch (reference)
      {
        case SnapReference.LeftEdge:
          return rect.x;
        case SnapReference.HorizontalCenter:
          return rect.center.x;
        case SnapReference.RightEdge:
          return rect.xMax;
        case SnapReference.TopEdge:
          return rect.y;
        case SnapReference.VerticalCenter:
          return rect.center.y;
        case SnapReference.BottomEdge:
          return rect.yMax;
        default:
          return 0.0f;
      }
    }

    public virtual void BeginSnap(List<Rect> snappableRects)
    {
      this.active = !this.active
        ? true
        : throw new InvalidOperationException("SnapService.BeginSnap: Already active. Call EndSnap() first.");
      this.m_SnappableRects = new List<Rect>((IEnumerable<Rect>)snappableRects);
    }

    public void UpdateSnapRects(List<Rect> snappableRects) => this.m_SnappableRects = snappableRects;

    public Rect GetSnappedRect(Rect sourceRect, out List<SnapResult> results, float scale = 1f)
    {
      if (!this.active)
        throw new InvalidOperationException("SnapService.GetSnappedRect: Already active. Call BeginSnap() first.");
      Rect r1 = sourceRect;
      this.m_CurrentScale = scale;
      results = this.GetClosestSnapElements(sourceRect);
      foreach (SnapResult result in results)
        this.ApplyResult(sourceRect, ref r1, result);
      foreach (SnapResult snapResult in results)
        snapResult.indicatorLine = this.GetSnapLine(r1, snapResult.sourceReference, snapResult.snappableRect,
          snapResult.snappableReference);
      return r1;
    }

    public virtual void EndSnap()
    {
      if (!this.active)
        throw new InvalidOperationException("SnapService.End: Already active. Call BeginSnap() first.");
      this.m_SnappableRects.Clear();
      this.active = false;
    }

    private SnapResult GetClosestSnapElement(
      Rect sourceRect,
      SnapReference sourceRef,
      Rect snappableRect,
      SnapReference startReference,
      SnapReference centerReference,
      SnapReference endReference)
    {
      float pos = SnapService.GetPos(sourceRect, sourceRef);
      float num1 = pos - SnapService.GetPos(snappableRect, startReference);
      float num2 = pos - SnapService.GetPos(snappableRect, endReference);
      float num3 = num1;
      SnapReference snapReference = startReference;
      if ((double)Math.Abs(num3) > (double)Math.Abs(num2))
      {
        num3 = num2;
        snapReference = endReference;
      }

      SnapResult snapResult = new SnapResult()
      {
        sourceRect = sourceRect,
        sourceReference = sourceRef,
        snappableRect = snappableRect,
        snappableReference = snapReference,
        offset = num3
      };
      return (double)snapResult.distance <= (double)this.snapDistance * 1.0 / (double)this.m_CurrentScale
        ? snapResult
        : (SnapResult)null;
    }

    private SnapResult GetClosestSnapElement(
      Rect sourceRect,
      SnapReference sourceRef,
      SnapReference startReference,
      SnapReference centerReference,
      SnapReference endReference)
    {
      SnapResult closestSnapElement1 = (SnapResult)null;
      float num = float.MaxValue;
      foreach (Rect snappableRect in this.m_SnappableRects)
      {
        SnapResult closestSnapElement2 = this.GetClosestSnapElement(sourceRect, sourceRef, snappableRect,
          startReference, centerReference, endReference);
        if (closestSnapElement2 != null && (double)num > (double)closestSnapElement2.distance)
        {
          num = closestSnapElement2.distance;
          closestSnapElement1 = closestSnapElement2;
        }
      }

      return closestSnapElement1;
    }

    private List<SnapResult> GetClosestSnapElements(Rect sourceRect,
      UnityEditor.Experimental.GraphView.Orientation orientation)
    {
      SnapReference snapReference1 = orientation == UnityEditor.Experimental.GraphView.Orientation.Horizontal
        ? SnapReference.LeftEdge
        : SnapReference.TopEdge;
      SnapReference snapReference2 = orientation == UnityEditor.Experimental.GraphView.Orientation.Horizontal
        ? SnapReference.HorizontalCenter
        : SnapReference.VerticalCenter;
      SnapReference snapReference3 = orientation == UnityEditor.Experimental.GraphView.Orientation.Horizontal
        ? SnapReference.RightEdge
        : SnapReference.BottomEdge;
      List<SnapResult> closestSnapElements = new List<SnapResult>(3);
      SnapResult closestSnapElement1 =
        this.GetClosestSnapElement(sourceRect, snapReference1, snapReference1, snapReference2, snapReference3);
      if (closestSnapElement1 != null)
        closestSnapElements.Add(closestSnapElement1);
      SnapResult closestSnapElement2 =
        this.GetClosestSnapElement(sourceRect, snapReference2, snapReference1, snapReference2, snapReference3);
      if (closestSnapElement2 != null)
        closestSnapElements.Add(closestSnapElement2);
      SnapResult closestSnapElement3 =
        this.GetClosestSnapElement(sourceRect, snapReference3, snapReference1, snapReference2, snapReference3);
      if (closestSnapElement3 != null)
        closestSnapElements.Add(closestSnapElement3);
      if (closestSnapElements.Count > 0)
      {
        closestSnapElements.Sort((Comparison<SnapResult>)((a, b) => a.distance.CompareTo(b.distance)));
        float minDistance = closestSnapElements[0].distance;
        closestSnapElements.RemoveAll((Predicate<SnapResult>)(r =>
          (double)Math.Abs(r.distance - minDistance) > 0.009999999776482582));
      }

      return closestSnapElements;
    }

    private List<SnapResult> GetClosestSnapElements(Rect sourceRect) => this
      .GetClosestSnapElements(sourceRect, UnityEditor.Experimental.GraphView.Orientation.Horizontal)
      .Union<SnapResult>((IEnumerable<SnapResult>)this.GetClosestSnapElements(sourceRect,
        UnityEditor.Experimental.GraphView.Orientation.Vertical)).ToList<SnapResult>();

    private Line2 GetSnapLine(Rect r, SnapReference reference)
    {
      Vector2 start = Vector2.zero;
      Vector2 end = Vector2.zero;
      switch (reference)
      {
        case SnapReference.LeftEdge:
          start = r.position;
          end = new Vector2(r.x, r.yMax);
          break;
        case SnapReference.HorizontalCenter:
          start = r.center;
          end = start;
          break;
        case SnapReference.RightEdge:
          start = new Vector2(r.xMax, r.yMin);
          end = new Vector2(r.xMax, r.yMax);
          break;
        case SnapReference.TopEdge:
          start = r.position;
          end = new Vector2(r.xMax, r.yMin);
          break;
        case SnapReference.VerticalCenter:
          start = r.center;
          end = start;
          break;
        default:
          start = new Vector2(r.x, r.yMax);
          end = new Vector2(r.xMax, r.yMax);
          break;
      }

      return new Line2(start, end);
    }

    private Line2 GetSnapLine(
      Rect r1,
      SnapReference reference1,
      Rect r2,
      SnapReference reference2)
    {
      bool flag = reference1 <= SnapReference.RightEdge;
      Line2 snapLine1 = this.GetSnapLine(r1, reference1);
      Line2 snapLine2 = this.GetSnapLine(r2, reference2);
      Vector2 start1 = snapLine1.start;
      Vector2 end1 = snapLine1.end;
      Vector2 start2 = snapLine2.start;
      Vector2 end2 = snapLine2.end;
      Vector2 start3 = Vector2.zero;
      Vector2 end3 = Vector2.zero;
      if (flag)
      {
        float x = start2.x;
        float y1 = Math.Min(end2.y, Math.Min(start2.y, Math.Min(start1.y, end1.y)));
        float y2 = Math.Max(end2.y, Math.Max(start2.y, Math.Max(start1.y, end1.y)));
        start3 = new Vector2(x, y1);
        end3 = new Vector2(x, y2);
      }
      else
      {
        float y = end2.y;
        float x1 = Math.Min(end2.x, Math.Min(start2.x, Math.Min(start1.x, end1.x)));
        float x2 = Math.Max(end2.x, Math.Max(start2.x, Math.Max(start1.x, end1.x)));
        start3 = new Vector2(x1, y);
        end3 = new Vector2(x2, y);
      }

      return new Line2(start3, end3);
    }

    private void ApplyResult(Rect sourceRect, ref Rect r1, SnapResult result)
    {
      if (result.snappableReference <= SnapReference.RightEdge)
        r1.x = sourceRect.x - result.offset;
      else
        r1.y = sourceRect.y - result.offset;
    }
  }
}
