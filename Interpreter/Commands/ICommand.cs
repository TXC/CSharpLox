namespace CodeCrafters.Interpreter.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Exit code</returns>
        void Execute(params string[] args);
    }
}
