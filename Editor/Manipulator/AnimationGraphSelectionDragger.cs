using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AnimationGraph.Editor
{
  /// <summary>
  ///   <para>Selection dragger manipulator.</para>
  /// </summary>
  public class AnimationGraphSelectionDragger : Dragger
  {
    private IDropTarget m_PrevDropTarget;
    private bool m_ShiftClicked = false;
    private bool m_Dragging = false;
    private Snapper m_Snapper = new Snapper();
    private GraphViewChange m_GraphViewChange;
    private List<GraphElement> m_MovedElements;
    private List<VisualElement> m_DropTargetPickList = new List<VisualElement>();
    private GraphView m_GraphView;
    private Dictionary<GraphElement, AnimationGraphSelectionDragger.OriginalPos> m_OriginalPos;
    private Vector2 m_originalMouse;
    internal const int k_PanAreaWidth = 100;
    internal const int k_PanSpeed = 4;
    internal const int k_PanInterval = 10;
    internal const float k_MinSpeedFactor = 0.5f;
    internal const float k_MaxSpeedFactor = 2.5f;
    internal const float k_MaxPanSpeed = 10f;
    private IVisualElementScheduledItem m_PanSchedule;
    private Vector3 m_PanDiff = Vector3.zero;
    private Vector3 m_ItemPanDiff = Vector3.zero;
    private Vector2 m_MouseDiff = Vector2.zero;
    private float m_XScale;

    internal bool snapEnabled { get; set; }

    private GraphElement selectedElement { get; set; }

    private GraphElement clickedElement { get; set; }

    private IDropTarget GetDropTargetAt(
      Vector2 mousePosition,
      IEnumerable<VisualElement> exclusionList)
    {
      Vector2 point = mousePosition;
      List<VisualElement> dropTargetPickList = this.m_DropTargetPickList;
      dropTargetPickList.Clear();
      this.target.panel.PickAll(point, dropTargetPickList);
      var dropTargetAt = (IDropTarget) null;
      for (int index = 0; index < dropTargetPickList.Count; ++index)
      {
        if (dropTargetPickList[index] != this.target || this.target == this.m_GraphView)
        {
          VisualElement visualElement = dropTargetPickList[index];
          if (visualElement is IDropTarget)
          {
            dropTargetAt = visualElement as IDropTarget;
            if (exclusionList.Contains<VisualElement>(visualElement))
              dropTargetAt = (IDropTarget) null;
            else
              break;
          }
        }
      }
      return dropTargetAt;
    }

    /// <summary>
    ///   <para>SelectionDragger's constructor.</para>
    /// </summary>
    public AnimationGraphSelectionDragger()
    {
      this.snapEnabled = EditorPrefs.GetBool("GraphSnapping", true);
      this.activators.Add(new ManipulatorActivationFilter()
      {
        button = MouseButton.LeftMouse
      });
      this.activators.Add(new ManipulatorActivationFilter()
      {
        button = MouseButton.LeftMouse,
        modifiers = EventModifiers.Shift
      });
      if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
        this.activators.Add(new ManipulatorActivationFilter()
        {
          button = MouseButton.LeftMouse,
          modifiers = EventModifiers.Command
        });
      else
        this.activators.Add(new ManipulatorActivationFilter()
        {
          button = MouseButton.LeftMouse,
          modifiers = EventModifiers.Control
        });
      this.panSpeed = new Vector2(1f, 1f);
      this.clampToParentEdges = false;
      this.m_MovedElements = new List<GraphElement>();
      this.m_GraphViewChange.movedElements = this.m_MovedElements;
    }

    /// <summary>
    ///   <para>Called to register click event callbacks on the target element.</para>
    /// </summary>
    protected override void RegisterCallbacksOnTarget()
    {
      if (!(this.target is ISelection))
        throw new InvalidOperationException("Manipulator can only be added to a control that supports selection");
      this.target.RegisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown));
      this.target.RegisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove));
      this.target.RegisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp));
      this.target.RegisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown));
      this.target.RegisterCallback<MouseCaptureOutEvent>(new EventCallback<MouseCaptureOutEvent>(this.OnMouseCaptureOutEvent));
      this.m_Dragging = false;
    }

    /// <summary>
    ///   <para>Called to unregister event callbacks from the target element.</para>
    /// </summary>
    protected override void UnregisterCallbacksFromTarget()
    {
      this.target.UnregisterCallback<MouseDownEvent>(new EventCallback<MouseDownEvent>(this.OnMouseDown));
      this.target.UnregisterCallback<MouseMoveEvent>(new EventCallback<MouseMoveEvent>(this.OnMouseMove));
      this.target.UnregisterCallback<MouseUpEvent>(new EventCallback<MouseUpEvent>(this.OnMouseUp));
      this.target.UnregisterCallback<KeyDownEvent>(new EventCallback<KeyDownEvent>(this.OnKeyDown));
      this.target.UnregisterCallback<MouseCaptureOutEvent>(new EventCallback<MouseCaptureOutEvent>(this.OnMouseCaptureOutEvent));
    }

    private static void SendDragAndDropEvent(
      IDragAndDropEvent evt,
      List<ISelectable> selection,
      IDropTarget dropTarget,
      ISelection dragSource)
    {
      if (dropTarget == null)
        return;
      EventBase eventBase = evt as EventBase;
      if (eventBase.eventTypeId == EventBase<DragExitedEvent>.TypeId())
        dropTarget.DragExited();
      else if (eventBase.eventTypeId == EventBase<DragEnterEvent>.TypeId())
        dropTarget.DragEnter(evt as DragEnterEvent, (IEnumerable<ISelectable>) selection, dropTarget, dragSource);
      else if (eventBase.eventTypeId == EventBase<DragLeaveEvent>.TypeId())
        dropTarget.DragLeave(evt as DragLeaveEvent, (IEnumerable<ISelectable>) selection, dropTarget, dragSource);
      if (!dropTarget.CanAcceptDrop(selection))
        return;
      if (eventBase.eventTypeId == EventBase<DragPerformEvent>.TypeId())
      {
        dropTarget.DragPerform(evt as DragPerformEvent, (IEnumerable<ISelectable>) selection, dropTarget, dragSource);
      }
      else
      {
        if (eventBase.eventTypeId != EventBase<DragUpdatedEvent>.TypeId())
          return;
        dropTarget.DragUpdated(evt as DragUpdatedEvent, (IEnumerable<ISelectable>) selection, dropTarget, dragSource);
      }
    }

    private void OnMouseCaptureOutEvent(MouseCaptureOutEvent e)
    {
      if (!this.m_Active)
        return;
      if (this.m_PrevDropTarget != null && this.m_GraphView != null && this.m_PrevDropTarget.CanAcceptDrop(this.m_GraphView.selection))
        this.m_PrevDropTarget.DragExited();
      this.selectedElement = (GraphElement) null;
      this.m_PrevDropTarget = (IDropTarget) null;
      this.m_Active = false;
      if (this.snapEnabled)
        this.m_Snapper.EndSnap(this.m_GraphView);
    }

    /// <summary>
    ///   <para>Called on mouse down event.</para>
    /// </summary>
    /// <param name="e">The event.</param>
    protected new void OnMouseDown(MouseDownEvent e)
    {
      if (this.m_Active)
      {
        e.StopImmediatePropagation();
      }
      else
      {
        if (!this.CanStartManipulation((IMouseEvent) e))
          return;
        this.m_GraphView = this.target as GraphView;
        if (this.m_GraphView == null)
          return;
        this.selectedElement = (GraphElement) null;
        this.clickedElement = e.target as GraphElement;
        if (this.clickedElement == null)
        {
          this.clickedElement = (e.target as VisualElement).GetFirstAncestorOfType<GraphElement>();
          if (this.clickedElement == null)
            return;
        }
        if (!this.clickedElement.IsMovable() || !this.clickedElement.HitTest(this.clickedElement.WorldToLocal(e.mousePosition)))
          return;
        if (!this.m_GraphView.selection.Contains((ISelectable) this.clickedElement))
        {
          e.StopImmediatePropagation();
        }
        else
        {
          this.selectedElement = this.clickedElement;
          this.m_OriginalPos = new Dictionary<GraphElement, AnimationGraphSelectionDragger.OriginalPos>();
          HashSet<GraphElement> graphElementSet = new HashSet<GraphElement>(this.m_GraphView.selection.OfType<GraphElement>());
          // foreach (Placemat placemat in new HashSet<Placemat>(graphElementSet.OfType<Placemat>()))
          //   placemat.GetElementsToMove(e.shiftKey, graphElementSet);
          foreach (GraphElement graphElement in graphElementSet)
          {
            if (graphElement != null && graphElement.IsMovable())
            {
              StackNode stackNode = (StackNode) null;
              if (graphElement.parent is StackNode)
              {
                stackNode = graphElement.parent as StackNode;
                if (stackNode.IsSelected((VisualElement) this.m_GraphView))
                  continue;
              }
              Rect position = graphElement.GetPosition();
              Rect rect = graphElement.hierarchy.parent.ChangeCoordinatesTo(this.m_GraphView.contentViewContainer, position);
              this.m_OriginalPos[graphElement] = new AnimationGraphSelectionDragger.OriginalPos()
              {
                pos = rect,
                scope = graphElement.GetContainingScope(),
                stack = stackNode,
                stackIndex = stackNode != null ? stackNode.IndexOf((VisualElement) graphElement) : -1
              };
            }
          }
          this.m_originalMouse = e.mousePosition;
          this.m_ItemPanDiff = Vector3.zero;
          if (this.m_PanSchedule == null)
          {
            this.m_PanSchedule = this.m_GraphView.schedule.Execute(new Action<TimerState>(this.Pan)).Every(10L).StartingIn(10L);
            this.m_PanSchedule.Pause();
          }
          this.snapEnabled = (this.selectedElement.capabilities & Capabilities.Snappable) != (Capabilities) 0 && EditorPrefs.GetBool("GraphSnapping", true);
          if (this.snapEnabled)
            this.m_Snapper.BeginSnap(this.m_GraphView);
          this.m_Active = true;
          this.target.CaptureMouse();
          e.StopImmediatePropagation();
        }
      }
    }

    internal Vector2 GetEffectivePanSpeed(Vector2 mousePos)
    {
      Vector2 vector = Vector2.zero;
      if ((double) mousePos.x <= 100.0)
        vector.x = (float) (-((100.0 - (double) mousePos.x) / 100.0 + 0.5) * 4.0);
      else if ((double) mousePos.x >= (double) this.m_GraphView.contentContainer.layout.width - 100.0)
        vector.x = (float) ((((double) mousePos.x - ((double) this.m_GraphView.contentContainer.layout.width - 100.0)) / 100.0 + 0.5) * 4.0);
      if ((double) mousePos.y <= 100.0)
        vector.y = (float) (-((100.0 - (double) mousePos.y) / 100.0 + 0.5) * 4.0);
      else if ((double) mousePos.y >= (double) this.m_GraphView.contentContainer.layout.height - 100.0)
        vector.y = (float) ((((double) mousePos.y - ((double) this.m_GraphView.contentContainer.layout.height - 100.0)) / 100.0 + 0.5) * 4.0);
      vector = Vector2.ClampMagnitude(vector, 10f);
      return vector;
    }

    private void ComputeSnappedRect(ref Rect selectedElementProposedGeom, float scale)
    {
      Rect snappedRect = this.m_Snapper.GetSnappedRect(this.selectedElement.parent.ChangeCoordinatesTo(this.m_GraphView.contentViewContainer, selectedElementProposedGeom), scale);
      selectedElementProposedGeom = this.m_GraphView.contentViewContainer.ChangeCoordinatesTo(this.selectedElement.parent, snappedRect);
    }

    /// <summary>
    ///   <para>Called on mouse move event.</para>
    /// </summary>
    /// <param name="e">The event.</param>
    protected new void OnMouseMove(MouseMoveEvent e)
    {
      if (!this.m_Active || this.m_GraphView == null)
        return;
      this.m_PanDiff = (Vector3) this.GetEffectivePanSpeed(((VisualElement) e.target).ChangeCoordinatesTo(this.m_GraphView.contentContainer, e.localMousePosition));
      if (this.m_PanDiff != Vector3.zero)
        this.m_PanSchedule.Resume();
      else
        this.m_PanSchedule.Pause();
      this.m_MouseDiff = this.m_originalMouse - e.mousePosition;
      Dictionary<Group, List<GraphElement>> dictionary = e.shiftKey ? new Dictionary<Group, List<GraphElement>>() : (Dictionary<Group, List<GraphElement>>) null;
      Rect selectedElementGeom = this.GetSelectedElementGeom();
      this.m_ShiftClicked = e.shiftKey;
      if (this.snapEnabled && !this.m_ShiftClicked)
        this.ComputeSnappedRect(ref selectedElementGeom, this.m_XScale);
      if (this.snapEnabled && this.m_ShiftClicked)
        this.m_Snapper.ClearSnapLines();
      foreach (KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos> originalPo in this.m_OriginalPos)
      {
        GraphElement key = originalPo.Key;
        if (key.hierarchy.parent != null)
        {
          if (!originalPo.Value.dragStarted)
          {
            key.GetFirstAncestorOfType<StackNode>()?.OnStartDragging(key);
            if (dictionary != null && key.GetContainingScope() is Group containingScope)
            {
              if (!dictionary.ContainsKey(containingScope))
                dictionary[containingScope] = new List<GraphElement>();
              dictionary[containingScope].Add(key);
            }
            originalPo.Value.dragStarted = true;
          }
          this.SnapOrMoveElement(originalPo, selectedElementGeom);
        }
      }
      if (dictionary != null)
      {
        // foreach (KeyValuePair<Group, List<GraphElement>> keyValuePair in dictionary)
        //   keyValuePair.Key.OnStartDragging((IMouseEvent) e, (IEnumerable<GraphElement>) keyValuePair.Value);
      }
      List<ISelectable> selection = this.m_GraphView.selection;
      IDropTarget dropTargetAt = this.GetDropTargetAt(e.mousePosition, selection.OfType<VisualElement>());
      if (this.m_PrevDropTarget != dropTargetAt)
      {
        if (this.m_PrevDropTarget != null)
        {
          using (DragLeaveEvent pooled = MouseEventBase<DragLeaveEvent>.GetPooled((IMouseEvent) e))
            AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, this.m_PrevDropTarget, (ISelection) this.m_GraphView);
        }
        using (DragEnterEvent pooled = MouseEventBase<DragEnterEvent>.GetPooled((IMouseEvent) e))
          AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, dropTargetAt, (ISelection) this.m_GraphView);
      }
      using (DragUpdatedEvent pooled = MouseEventBase<DragUpdatedEvent>.GetPooled((IMouseEvent) e))
        AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, dropTargetAt, (ISelection) this.m_GraphView);
      this.m_PrevDropTarget = dropTargetAt;
      this.m_Dragging = true;
      e.StopPropagation();
    }

    private void Pan(TimerState ts)
    {
      this.m_GraphView.viewTransform.position -= this.m_PanDiff;
      this.m_ItemPanDiff += this.m_PanDiff;
      Rect selectedElementGeom = this.GetSelectedElementGeom();
      if (this.snapEnabled && !this.m_ShiftClicked)
        this.ComputeSnappedRect(ref selectedElementGeom, this.m_XScale);
      foreach (KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos> originalPo in this.m_OriginalPos)
        this.SnapOrMoveElement(originalPo, selectedElementGeom);
    }

    private void SnapOrMoveElement(
      KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos> v,
      Rect selectedElementGeom)
    {
      GraphElement key = v.Key;
      if (EditorPrefs.GetBool("GraphSnapping"))
      {
        Vector2 vector2 = selectedElementGeom.position - this.m_OriginalPos[this.selectedElement].pos.position;
        Rect position = key.GetPosition();
        key.SetPosition(new Rect(v.Value.pos.x + vector2.x, v.Value.pos.y + vector2.y, position.width, position.height));
      }
      else
        this.MoveElement(key, v.Value.pos);
    }

    private Rect GetSelectedElementGeom()
    {
      this.m_XScale = this.selectedElement.worldTransform.m00;
      Rect pos = this.m_OriginalPos[this.selectedElement].pos;
      pos.x -= (this.m_MouseDiff.x - this.m_ItemPanDiff.x) * this.panSpeed.x / this.m_XScale;
      pos.y -= (this.m_MouseDiff.y - this.m_ItemPanDiff.y) * this.panSpeed.y / this.m_XScale;
      return pos;
    }

    private void MoveElement(GraphElement element, Rect originalPos)
    {
      Matrix4x4 worldTransform = element.worldTransform;
      Vector3 vector3 = new Vector3(worldTransform.m00, worldTransform.m11, worldTransform.m22);
      element.SetPosition(this.m_GraphView.contentViewContainer.ChangeCoordinatesTo(element.hierarchy.parent, new Rect(0.0f, 0.0f, originalPos.width, originalPos.height)
      {
        x = originalPos.x - (this.m_MouseDiff.x - this.m_ItemPanDiff.x) * this.panSpeed.x / vector3.x * element.transform.scale.x,
        y = originalPos.y - (this.m_MouseDiff.y - this.m_ItemPanDiff.y) * this.panSpeed.y / vector3.y * element.transform.scale.y
      }));
    }

    /// <summary>
    ///   <para>Called on mouse up event.</para>
    /// </summary>
    /// <param name="e">The event.</param>
    /// <param name="evt"></param>
    protected new void OnMouseUp(MouseUpEvent evt)
    {
      if (this.m_GraphView == null)
      {
        if (!this.m_Active)
          return;
        this.target.ReleaseMouse();
        this.selectedElement = (GraphElement) null;
        this.m_Active = false;
        this.m_Dragging = false;
        this.m_PrevDropTarget = (IDropTarget) null;
      }
      else
      {
        List<ISelectable> selection = this.m_GraphView.selection;
        if (!this.CanStopManipulation((IMouseEvent) evt))
          return;
        if (this.m_Active)
        {
          if (this.m_Dragging)
          {
            if (m_GraphView is AnimationGraphView animationGraphView)
            {
              var moveElementCommand = new MoveElementCommand(animationGraphView, m_OriginalPos);
              animationGraphView.PushNewCommand(moveElementCommand);
            }

            foreach (IGrouping<StackNode, GraphElement> collection in this.m_OriginalPos.GroupBy<KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos>, StackNode, GraphElement>((Func<KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos>, StackNode>) (v => v.Value.stack), (Func<KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos>, GraphElement>) (v => v.Key)))
            {
              if (collection.Key != null && this.m_GraphView.elementsRemovedFromStackNode != null)
                this.m_GraphView.elementsRemovedFromStackNode(collection.Key, (IEnumerable<GraphElement>) collection);
              foreach (GraphElement graphElement in (IEnumerable<GraphElement>) collection)
                graphElement.UpdatePresenterPosition();
              this.m_MovedElements.AddRange((IEnumerable<GraphElement>) collection);
            }
            if (this.target is UnityEditor.Experimental.GraphView.GraphView target && target.graphViewChanged != null)
            {
              KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos> keyValuePair = this.m_OriginalPos.First<KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos>>();
              this.m_GraphViewChange.moveDelta = keyValuePair.Key.GetPosition().position - keyValuePair.Value.pos.position;
              GraphViewChange graphViewChange = target.graphViewChanged(this.m_GraphViewChange);
            }
            this.m_MovedElements.Clear();
          }
          this.m_PanSchedule.Pause();
          if (this.m_ItemPanDiff != Vector3.zero)
            this.m_GraphView.UpdateViewTransform(this.m_GraphView.contentViewContainer.transform.position, this.m_GraphView.contentViewContainer.transform.scale);
          if (selection.Count > 0 && this.m_PrevDropTarget != null)
          {
            if (this.m_PrevDropTarget.CanAcceptDrop(selection))
            {
              using (DragPerformEvent pooled = MouseEventBase<DragPerformEvent>.GetPooled((IMouseEvent) evt))
                AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, this.m_PrevDropTarget, (ISelection) this.m_GraphView);
            }
            else
            {
              using (DragExitedEvent pooled = MouseEventBase<DragExitedEvent>.GetPooled((IMouseEvent) evt))
                AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, this.m_PrevDropTarget, (ISelection) this.m_GraphView);
            }
          }
          if (this.snapEnabled)
            this.m_Snapper.EndSnap(this.m_GraphView);
          this.target.ReleaseMouse();
          evt.StopPropagation();
        }
        this.selectedElement = (GraphElement) null;
        this.m_Active = false;
        this.m_PrevDropTarget = (IDropTarget) null;
        this.m_Dragging = false;
        this.m_PrevDropTarget = (IDropTarget) null;
      }
    }

    private void OnKeyDown(KeyDownEvent e)
    {
      if (e.keyCode != KeyCode.Escape || this.m_GraphView == null || !this.m_Active)
        return;
      Dictionary<Scope, List<GraphElement>> dictionary = new Dictionary<Scope, List<GraphElement>>();
      foreach (KeyValuePair<GraphElement, AnimationGraphSelectionDragger.OriginalPos> originalPo in this.m_OriginalPos)
      {
        AnimationGraphSelectionDragger.OriginalPos originalPos = originalPo.Value;
        if (originalPos.stack != null)
        {
          originalPos.stack.InsertElement(originalPos.stackIndex, originalPo.Key);
        }
        else
        {
          if (originalPos.scope != null)
          {
            if (!dictionary.ContainsKey(originalPos.scope))
              dictionary[originalPos.scope] = new List<GraphElement>();
            dictionary[originalPos.scope].Add(originalPo.Key);
          }
          originalPo.Key.SetPosition(originalPos.pos);
        }
      }
      foreach (KeyValuePair<Scope, List<GraphElement>> keyValuePair in dictionary)
        keyValuePair.Key.AddElements((IEnumerable<GraphElement>) keyValuePair.Value);
      this.m_PanSchedule.Pause();
      if (this.m_ItemPanDiff != Vector3.zero)
        this.m_GraphView.UpdateViewTransform(this.m_GraphView.contentViewContainer.transform.position, this.m_GraphView.contentViewContainer.transform.scale);
      using (DragExitedEvent pooled = EventBase<DragExitedEvent>.GetPooled())
      {
        List<ISelectable> selection = this.m_GraphView.selection;
        AnimationGraphSelectionDragger.SendDragAndDropEvent((IDragAndDropEvent) pooled, selection, this.m_PrevDropTarget, (ISelection) this.m_GraphView);
      }
      this.target.ReleaseMouse();
      e.StopPropagation();
    }

    internal class OriginalPos
    {
      public Rect pos;
      public Scope scope;
      public StackNode stack;
      public int stackIndex;
      public bool dragStarted;
    }
  }
}
