namespace CodeCrafters.Interpreter.Commands
{
    public abstract class BaseCommand
    {
        public BaseCommand()
        {

        }

        public string ParseFile(params string[] args)
        {
            var size = args.Count();
            var source = args[size - 1];
            if (!File.Exists(source))
            {
                throw new FileNotFoundException("File not found", source);
            }

            return File.ReadAllText(source).TrimEnd();
        }

        //public int Execute()
        //{
        //    throw new NotImplementedException();
        //}


    }
}
