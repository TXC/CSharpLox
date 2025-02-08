namespace CodeCrafters.Interpreter.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns>Exit code</returns>
        int Execute(params string[] args);
    }
}
