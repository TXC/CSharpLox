using CodeCrafters.Interpreter.Attributes;

namespace CodeCrafters.Interpreter.Commands;

[Command("tokenize")]
public class Tokenize : ICommand
{
    public Tokenize() { }

    public void Execute(params string[] args)
    {
        var filename = args[1];
        //var fileContents = File.ReadAllText(Path.Combine("Data", filename));
        var fileContents = "";

        //// You can use print statements as follows for debugging, they'll be visible when running tests.
        //Console.Error.WriteLine("Logs from your program will appear here!");

        //Uncomment this block to pass the first stage
        if (!string.IsNullOrEmpty(fileContents))
        {
            throw new NotImplementedException("Scanner not implemented");
        }
        else
        {
            Console.WriteLine("EOF  null"); // Placeholder, remove this line when implementing the scanner
        }
    }
}
