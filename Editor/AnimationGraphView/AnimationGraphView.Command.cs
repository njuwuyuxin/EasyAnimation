using System.Collections.Generic;

namespace AnimationGraph.Editor
{
    public partial class AnimationGraphView
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        internal void PushNewCommand(ICommand command)
        {
            command.Do();
            undoStack.Push(command);
            redoStack.Clear();
        }
        
        private void TryUndoCommand()
        {
            if (undoStack.TryPop(out var command))
            {
                command.Undo();
                redoStack.Push(command);
            }
        }

        private void TryRedoCommand()
        {
            if (redoStack.TryPop(out var command))
            {
                command.Redo();
                undoStack.Push(command);
            }
        }
    }
}
