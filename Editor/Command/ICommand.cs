namespace AnimationGraph.Editor
{
    public interface ICommand
    {
        public void Do();
        public void Undo();
        public void Redo();
    }
}