namespace CodeCrafters.Interpreter.Commands
{
    public interface ICommand
    {
        void Execute(params string[] args);
    }
}
